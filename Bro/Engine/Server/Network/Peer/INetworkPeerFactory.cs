using Bro.Network;

namespace Bro.Server.Network
{
    public interface INetworkPeerFactory
    {
        PeerPair CreatePeer(ConnectionProtocol protocol, ITransportPeer transportPeer); // add peerData to constructor of Tcp/UdpPeer
    }
}