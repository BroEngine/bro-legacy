using System;
using Bro.Network;

namespace Bro.Server.Network.Offline
{
    public class OfflineNetworkPeer : INetworkPeer
    {   
        public Action<int> OnDisconnectHandler { get; set; }
        public Action OnConnectHandler { get; set; }
        public Action<byte[]> OnReceiveBinaryHandler { get; set; }
        
        public int Id => 1;

        public OfflineNetworkPeer(ITransportPeer transportPeer)
        {
        }
        
        public void OnConnect()
        {
            OnConnectHandler?.Invoke();
        }
        
        public void Disconnect(int code)
        {
            OnDisconnectHandler?.Invoke( code );
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            OfflineBridge.ServerDisconnectClient(code);
#endif
        }

        public void Send(byte[] data, bool isReliable, bool isOrdered)
        {
            throw new NotImplementedException();
        }
        
        public void Send(string data, bool isReliable, bool isOrdered)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(string data)
        {
            throw new NotImplementedException();
        }
        
        public void OnReceive(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}