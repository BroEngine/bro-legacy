using System;
using System.Diagnostics;
using Bro.Network;

#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
namespace Bro.Client.Network
{
    public class OfflineClientPeer : IClientPeer
    {
        /* statistic */

        public long LastPacketElapsedMs => _receiveTimer.ElapsedMilliseconds;
        public long TotalSent { get; private set; }

        public long TotalReceived { get; private set; }
        /* statistic */

        private bool _connected = false;
        private readonly Stopwatch _receiveTimer = new Stopwatch();
        private readonly Stopwatch _sendTimer = new Stopwatch();
        private readonly Stopwatch _inactivityTimer;

        public OfflineClientPeer(IConnectionConfig config)
        {
            _inactivityTimer = new Stopwatch();
            _inactivityTimer.Start();
        }

        public long InactivityTime { get { return _inactivityTimer.ElapsedMilliseconds; } }

        public Action<INetworkOperation> OnReceiveNetworkEvent { get; set; }
        public Action<INetworkOperation> OnReceiveNetworkResponse { get; set; }
        public Action<INetworkOperation> OnReceiveLetsEncryptOperation { get; set; }
        public Action<INetworkOperation> OnReceivePingOperation { get; set; }
        public Action<INetworkOperation> OnReceiveHandShakeOperation { get; set; }
        public Action<int> OnDisconnect { get; set; }

        public Action OnConnect { get; set; }


        public bool Connect()
        {
            if (OfflineBridge.ClientConnect(this))
            {
                _connected = true;
                OnConnect?.Invoke();
            }
            else
            {
                _connected = false;
                OnDisconnect?.Invoke(DisconnectCode.Undefined);
            }

            return _connected;
        }

        public void Disconnect(int code)
        {
            if (!_connected)
            {
                return;
            }

            _connected = false;

            OfflineBridge.ClientDisconnectFromServer(this);

            OnDisconnect?.Invoke(code);
        }

        public void OnDisconnectFromServer(int code)
        {
            if (!_connected)
            {
                return;
            }

            _connected = false;

            OnDisconnect?.Invoke(code);
        }

        public void Dispose()
        {
            OnReceiveNetworkEvent = null;
            OnReceiveNetworkResponse = null;
            OnReceiveLetsEncryptOperation = null;
            OnReceivePingOperation = null;
            OnReceiveHandShakeOperation = null;
        }

        public void SetEncryption(string key)
        {
            Bro.Log.Info("Client OfflinePeer received encryption key = " + key);
        }

        public void Send(INetworkOperation operation)
        {
            _sendTimer.Reset();
            _sendTimer.Start();
            ++TotalSent;

            OfflineBridge.ClientSendOperation(operation);
        }

        public void Receive(INetworkOperation operation)
        {
            operation.Retain();
            _receiveTimer.Reset();
            _receiveTimer.Start();
            ++TotalReceived;

            _inactivityTimer.Reset();
            _inactivityTimer.Start();

            switch (operation.Type)
            {
                case NetworkOperationType.Event:
                    OnReceiveNetworkEvent(operation);
                    break;
                case NetworkOperationType.Response:
                    OnReceiveNetworkResponse(operation);
                    break;
                case NetworkOperationType.Request:
                    Bro.Log.Error("Received request operation, client not handle requests!");
                    break;
                case NetworkOperationType.Ping:
                    OnReceivePingOperation(operation);
                    break;
                case NetworkOperationType.Encryption:
                    OnReceiveLetsEncryptOperation(operation);
                    break;
                case NetworkOperationType.Handshake:
                    OnReceiveHandShakeOperation(operation);
                    break;
                default:
                    throw new Exception("Cannot handle with " + operation.Type);
            }
            operation.Release();
        }
    }
}
#endif