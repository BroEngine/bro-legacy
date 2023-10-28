using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class LogMonitor : StaticSingleton<LogMonitor>
    {       
        private readonly StringBuilder _logData = new StringBuilder ();
     
        public LogMonitor ()
        {            
            _logData.AppendLine ( string.Format ( "Version: {0}\r\n", Application.version ) );
            _logData.AppendLine ( string.Format ( "OS: {0}\r\n", SystemInfo.operatingSystem ) );
            _logData.AppendLine ( string.Format ( "Device: {0}\r\n", SystemInfo.deviceModel ) );
            _logData.AppendLine ( string.Format ( "Processor: {0}\r\n", SystemInfo.processorType ) );
            _logData.AppendLine ( string.Format ( "Memory: {0} Mb\r\n", SystemInfo.systemMemorySize ) );
            
            Application.logMessageReceived += OnLogCallBack;
        }
        
        private void OnLogCallBack (string condition, string stackTrace, UnityEngine.LogType type)
        {
            var msg = type + " " + DateTime.Now + " Message: " + condition;

            if (!string.IsNullOrEmpty(stackTrace))
            {
                msg = msg + Environment.NewLine + " Stack:" + stackTrace + Environment.NewLine;
            }

            LogHandler ( msg );
            
            if ( UnityEngine.LogType.Assert == type || UnityEngine.LogType.Exception == type || UnityEngine.LogType.Error == type )
            {
                LogHandler ( Environment.NewLine + Environment.StackTrace + Environment.NewLine  );
            } 
        }

        private void LogHandler (string entry)
        {
            _logData.AppendLine(entry);
        }

        private void WriteLogLineToStaticLog (string msg)
        {
            File.AppendAllText ( LogPath, msg );
        }

        private void ResetLogFile()
        {            
            var filePath = LogPath;
            var dirPath = Path.GetDirectoryName(filePath);     
           
            if (!Directory.Exists(dirPath)) 
            {
                Directory.CreateDirectory(dirPath);
            }
            
            File.WriteAllText ( filePath, string.Empty );
        }
 
        private string LogPath => Path.Combine ( Application.persistentDataPath, "staticlog.txt" );

        public string Flush()
        {
            ResetLogFile();

            var path = LogPath;
            var text = _logData.ToString();
            
            File.WriteAllText ( path, text );

            return path;
        }
    }
}