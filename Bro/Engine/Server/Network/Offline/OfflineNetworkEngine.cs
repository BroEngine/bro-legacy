#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )

using Bro.Network;

namespace Bro.Server.Network.Offline
{
    public class OfflineNetworkEngine
    {
        private readonly INetworkPeerFactory _peerFactory;
        private OfflineClientPeer _peer;

        public OfflineNetworkEngine(INetworkPeerFactory peerFactory)
        {
            _peerFactory = peerFactory;

            OfflineBridge.CreateServerPeer += OnOfflinePeerConnected;
        }

        public void Stop()
        {
        }

        private Server.Network.OfflineClientPeer OnOfflinePeerConnected(OfflineTransportPeer transportPeer)
        {
            var peerPair = _peerFactory.CreatePeer(ConnectionProtocol.Offline, transportPeer);

            peerPair.NetworkPeer.OnConnect();

            _peer = (OfflineClientPeer) peerPair.ClientPeer;
            return _peer;
        }
    }
}
#endif