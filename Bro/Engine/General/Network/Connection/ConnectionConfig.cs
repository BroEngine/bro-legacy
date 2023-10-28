using System;
using Bro.Network;

namespace Bro
{
    [System.Serializable]
    public class ConnectionConfig : IConnectionConfig
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public ConnectionProtocol Protocol { get; private set; }

        public ConnectionConfig(string ip, int port, ConnectionProtocol protocol)
        {
            Host = ip;
            Port = port;
            Protocol = protocol;
        }

        public override string ToString()
        {
            switch (Protocol)
            {
                case ConnectionProtocol.Undefined:
                    return $"unknown://{Host}:{Port}";

                case ConnectionProtocol.Udp:
                    return $"udp://{Host}:{Port}";

                case ConnectionProtocol.Tcp:
                    return $"tcp://{Host}:{Port}";

                case ConnectionProtocol.Offline:
                    return $"offline://{Host}:{Port}";

                default:
                    return $"{Host}:{Port}";
            }
        }
    }
}