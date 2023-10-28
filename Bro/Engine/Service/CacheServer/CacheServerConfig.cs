namespace Bro.Service
{
    public class CacheServerConfig
    {
        public readonly string ConfigurationGate;
        public readonly int Port;

        public CacheServerConfig(string gate, int port)
        {
            ConfigurationGate = gate;
            Port = port;
        }
    }
}