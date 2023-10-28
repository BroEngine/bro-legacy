using System;
using System.Diagnostics;
using Bro.Network;

namespace Bro.Client.Network
{
    public interface IClientPeer
    {
        /* statistic */
        long LastPacketElapsedMs { get; }
        long TotalSent { get; }
        long TotalReceived { get; }
        /* statistic */
        
        Action<INetworkOperation> OnReceiveNetworkEvent { set; }
        Action<INetworkOperation> OnReceiveNetworkResponse  { set; }
        Action<INetworkOperation> OnReceiveLetsEncryptOperation  { set; }
        Action<INetworkOperation> OnReceivePingOperation  { set; }
        Action<INetworkOperation> OnReceiveHandShakeOperation  { set; }
        Action<int> OnDisconnect { set; }
        Action OnConnect { set; }
        
        void Disconnect(int code);
        void Dispose();
        void SetEncryption(string key);
        void Send(INetworkOperation operation);
        bool Connect();

        long InactivityTime { get; }
    }
}