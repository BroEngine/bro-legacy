#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
using Bro.Network;

namespace Bro.Network
{
    public static class OfflineBridge
    {
        public delegate Server.Network.OfflineClientPeer CreateServerPeerDelegate(OfflineTransportPeer transportPeer);

        public static CreateServerPeerDelegate CreateServerPeer;
        private static OfflineTransportPeer _transportPeer;

        private static Server.Network.OfflineClientPeer _serverPeer;
        private static Client.Network.OfflineClientPeer _clientPeer;

        public static bool ClientConnect(Client.Network.OfflineClientPeer clientPeer)
        {
            if (CreateServerPeer == null)
            {
                return false;
            }

            _transportPeer = new OfflineTransportPeer();
            _serverPeer = CreateServerPeer(_transportPeer);
            _clientPeer = clientPeer;

            return _serverPeer != null;
        }

        public static void ServerDisconnectClient(int code)
        {
            _clientPeer?.OnDisconnectFromServer(code);
            _clientPeer = null;
            _transportPeer = null;
            _serverPeer = null;
        }

        public static void ClientDisconnectFromServer(Client.Network.OfflineClientPeer clientPeer)
        {
            _clientPeer = null;
            _transportPeer = null;

            if (_serverPeer != null)
            {
                _serverPeer.Disconnect(0);
                _serverPeer = null;
            }
        }

        public static void ClientSendOperation(INetworkOperation operation)
        {
            _serverPeer?.Receive(operation);
        }

        public static void ServerSendOperation(INetworkOperation operation)
        {
            _clientPeer?.Receive(operation);
        }
    }
}
#endif