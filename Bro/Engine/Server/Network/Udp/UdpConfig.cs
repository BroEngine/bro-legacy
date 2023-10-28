namespace Bro.Server.Network.Udp
{
    [System.Serializable]
    public class NetworkEngineConfig
    {
        public int Port;

        public int MaxConnections = 1024;
        public int BottleneckConnections = 32;
        public int Threads = 2;
        
    }
}