using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Bro
{
    public class SystemHealth
    {
        public class Health
        {
            public long Rx;
            public long Tx;
            public int CpuCores;
            public float CpuUsage;
            public long MemoryTotal;
            public float MemoryUsage;
            public int Connections;
            public long DiskTotal;
            public float DiskUsage;
        }
        
        private class Core
        {
            public long User;
            public long Nice;
            public long System;
            public long Idle;
        }
        
        private class Memory
        {
            public long Total;
            public float Usage;
        }
        
        private class Disk
        {
            public long Total;
            public float Usage;
        }
        
        
        private class Bandwidth
        {
            public long Rx; // input
            public long Tx; // output
        }
        
        private readonly Stopwatch _stopwatch;
        
        private readonly string _networkInterface;
        private readonly int _cpuCores;
        private readonly long _memoryTotal;

        private List<Core> _previousCores;
        private Bandwidth _previousBandwidth;
        
        public SystemHealth()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _networkInterface = GetNetworkInterface();
            _cpuCores = GetCores().Count;
            _memoryTotal = GetMemory().Total;

            _previousCores = GetCores();
            _previousBandwidth = GetBandwidth();
        }

        public Health Pop()
        {
            var timestamp = _stopwatch.ElapsedMilliseconds;

            var memory = GetMemory();
            var disk = GetDisk();
            var currentCores = GetCores();
            var currentBandwidth = GetBandwidth();

            var bandwidth = GetDeltaBandwidth(timestamp,_previousBandwidth, currentBandwidth);

            var health = new Health
            {
                Rx = bandwidth.Rx,
                Tx = bandwidth.Tx,
                CpuCores = _cpuCores,
                CpuUsage = (float) Math.Round( GetCpuUsage(_previousCores, currentCores), 3 ),
                MemoryTotal = _memoryTotal,
                MemoryUsage = (float) Math.Round( memory.Usage , 3 ),
                Connections = GetConnections(),
                DiskTotal = disk.Total,
                DiskUsage = (float) Math.Round( disk.Usage  , 3 )
            };

            _previousCores = currentCores;
            _previousBandwidth = currentBandwidth;
    
            _stopwatch.Stop();
            _stopwatch.Reset();
            _stopwatch.Start();
            return health;
        }

        private List<Core> GetCores()
        {
            var cores = new List<Core>();
            try
            {
                var statValue = File.ReadAllText( "/proc/stat" );
                using (var reader = new StringReader(statValue))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if( Regex.IsMatch(line,"cpu[0-9]", RegexOptions.IgnoreCase) )
                        {
                            var info = line.Split(' ');
                            var core = new Core()
                            {
                                User = long.Parse( info[1] ),
                                Nice = long.Parse( info[2] ),
                                System = long.Parse( info[3] ),
                                Idle = long.Parse( info[4] ),
                            };
                            cores.Add(core);
                        }    
                    }
                }
            }
            catch (Exception) { /* ignored */ }
            return cores;
        }
        
        private Memory GetMemory()
        {
            var memory = new Memory();
            try
            {
                var freeResult = Exec("free").Trim();
                var freeArray = freeResult.Split('\n');
                var freeString = Regex.Replace( freeArray[1], @"\s+", " ");
                var memoryArray = freeString.Split(' ');
                var used = long.Parse(memoryArray[2]) * 1024;
                memory.Total = long.Parse(memoryArray[1]) * 1024;
                memory.Usage = (float)( (double) used / memory.Total );
            }
            catch (Exception) { /* ignored */ }
            return memory;
        }
        
        private Disk GetDisk()
        {
            var disk = new Disk();
            try
            {
                var command = "df -B1 -t ext4 --output=size,used | grep -v 'Used'";
                var execResult = Exec( command ).Trim();
                var diskString = Regex.Replace( execResult, @"\s+", " ");
                var diskArray = diskString.Split(' ');

                var size = long.Parse(diskArray[0]);
                var used = long.Parse(diskArray[1]);

                disk.Total = size;
                disk.Usage = (float)( (double) used / size );
            }
            catch (Exception) { /* ignored */ }
            return disk;
        }
        
        
        private string GetNetworkInterface()
        {
            try
            {
                var networkFolders = Directory.GetDirectories(@"/sys/class/net/", "*", SearchOption.TopDirectoryOnly);
                foreach (var dir in networkFolders)
                {
                    var networkInterface = dir.Replace( "/sys/class/net/" , string.Empty );
                    if (networkInterface != "lo")
                    {
                        return networkInterface;
                    }
                }
            }
            catch (Exception) { /* ignored */ }
            return string.Empty;
        }
       
        private static string Exec(string command)
        {
            command = command.Replace("\"","\"\"");
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \""+ command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd();
        }
        
        private int GetConnections()
        {
            var connections = 0;
            try
            {
                var result = Exec("netstat -anp | grep ESTABLISHED | wc -l");
                connections = int.Parse(result);
            }
            catch (Exception) { /* ignored */ }
            return connections;
        }
        
        private Bandwidth GetBandwidth()
        {
            try
            {
                var rxFile = "/sys/class/net/" + _networkInterface + "/statistics/rx_bytes";
                var txFile = "/sys/class/net/" + _networkInterface + "/statistics/tx_bytes";
                var rxValue = long.Parse(File.ReadAllText(rxFile));
                var txValue = long.Parse(File.ReadAllText(txFile));
                return new Bandwidth() { Rx = (long) rxValue, Tx = (long) txValue };
            }
            catch (Exception) { /* ignored */ }
            return new Bandwidth();
        }
        
        private Bandwidth GetDeltaBandwidth(long timestamp, Bandwidth a, Bandwidth b)
        {
            try
            {
                var rxDelta = b.Rx - a.Rx;
                var txDelta = b.Tx - a.Tx;
                var rxPerSecond = rxDelta * ( 1000.0 / timestamp ); 
                var txPerSecond = txDelta * ( 1000.0 / timestamp );
                return new Bandwidth() { Rx = (long) rxPerSecond, Tx = (long) txPerSecond };
            }
            catch (Exception) { /* ignored */ }
            return new Bandwidth();
        }

        private float GetCpuUsage(List<Core> coresA, List<Core> coresB)
        {
            var userTotal = 0L;
            var cpuTotal = 0L;
            for (var i = 0; i < coresA.Count; ++i)
            {
                var userDelta = coresB[i].User - coresA[i].User;
                var niceDelta = coresB[i].Nice - coresA[i].Nice;
                var systemDelta = coresB[i].System - coresA[i].System;
                var idleDelta = coresB[i].Idle - coresA[i].Idle;
                cpuTotal += (userDelta + niceDelta + systemDelta + idleDelta);
                userTotal += userDelta;
            }

            if (cpuTotal != 0)
            {
                return (float) ((userTotal / (double) cpuTotal));
            }

            return 0.0f;
        }

        public static bool Available {
            get
            {
                try
                {
                    var name = Exec("uname").Trim();
                    return name == "Linux";
                }
                catch (Exception) { /* ignored */ }
                return false;
            }
        }
    }
}