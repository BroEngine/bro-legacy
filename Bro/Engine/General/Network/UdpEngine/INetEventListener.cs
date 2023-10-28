namespace Bro.Network.Udp.Engine
{
    public interface INetEventListener
    {
        void OnPeerConnected(NetPeer peer);
        void OnPeerDisconnected(NetPeer peer, int code);
        void OnNetworkReceive(NetPeer peer, byte[] data);
        void OnConnectionRequest(ConnectionRequest request);
    }
}
