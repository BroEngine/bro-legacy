using Bro.Threading;

namespace Bro.Server.Network
{
    public class NetworkEngine
    {
        [System.Serializable]
        public class Config
        {
            public int OperationThreads = 1;
            public int ReadThreads = 1;
            public int WriteThreads = 1;

            public Tcp.NetworkEngineConfig TcpConfig;
            public Udp.NetworkEngineConfig UdpConfig;
        }

        private readonly Tcp.NetworkEngine _tcpEngine;
        private readonly Udp.NetworkEngine _udpEngine;

        public NetworkEngine(INetworkPeerFactory peerFactory, Config config)
        {
            NetworkOperationThreadPool.ConfiguratePool(config.OperationThreads);
            NetworkWriteThreadPool.ConfiguratePool(config.WriteThreads);
            NetworkReadThreadPool.ConfiguratePool(config.ReadThreads);
            
            if (config.TcpConfig != null)
            {
                Bro.Log.Info("network engine :: tcp enabled");
                if (config.TcpConfig.Port == 0)
                {
                    Bro.Log.Error("network engine :: tcp port is 0");
                }
                _tcpEngine = new Tcp.NetworkEngine(peerFactory);
                _tcpEngine.Start(config.TcpConfig);
            }
            else
            {
                Bro.Log.Info("network engine :: tcp disabled");
            }

            if (config.UdpConfig  != null)
            {
                Bro.Log.Info("network engine :: udp enabled");
                if (config.UdpConfig.Port == 0)
                {
                    Bro.Log.Error("network engine :: udp port is 0");
                }
                _udpEngine = new Udp.NetworkEngine(peerFactory);
                _udpEngine.Start(config.UdpConfig);
            }
            else
            {
                Bro.Log.Info("network engine :: udp disabled");
            }
        }

        public void Stop()
        {
            if (_tcpEngine != null)
            {
                _tcpEngine.Stop();
            }

            if (_udpEngine != null)
            {
                _udpEngine.Stop();
            }
        }
        
    }
}