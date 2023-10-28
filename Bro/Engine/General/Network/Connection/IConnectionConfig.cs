using Bro.Network;

namespace Bro
{
    public interface IConnectionConfig
    {
        string Host { get; }
        int Port { get; }
        ConnectionProtocol Protocol { get; }
    }
}