namespace Bro.Server.Network
{
    public struct PeerPair
    {
        public readonly INetworkPeer NetworkPeer;
        public readonly IClientPeer ClientPeer;

        public PeerPair(INetworkPeer networkPeer, IClientPeer clientPeer)
        {
            NetworkPeer = networkPeer;
            ClientPeer = clientPeer;
        }
    }
}