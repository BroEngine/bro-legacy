using System;
using System.Reflection;
using Bro.Client;
using Bro.Engine;
using Bro.Network;

namespace Bro.Client.Network
{
    public class NetworkEngine
    {
        private IClientPeer _peer;
        private NetworkStatus _currentStatus;

        public event Action<NetworkStatus, int> OnStatusChanged;
        public event Action<LetsEncryptOperation> OnLetsEncryptOperationReceived;
        public event Action<HandShakeOperation> OnHandShakeOperationReceived;
        public event Action<PingOperation> OnPingOperationReceived;
        public event Action<INetworkEvent> OnNetworkEventReceived;
        public event Action<INetworkResponse> OnNetworkResponseReceived;

        public IConnectionConfig ConnectionConfig { get; private set; }

        public NetworkEngine()
        {
            _currentStatus = NetworkStatus.Disconnected;
        }

        private void SetStatus(NetworkStatus status, int descriptionCode = 0)
        {
            if (_currentStatus != status)
            {
                Log.Info($"network engine :: changed status from {_currentStatus} to {status} descriptionCode ={descriptionCode}");
                _currentStatus = status;
                OnStatusChanged?.Invoke(_currentStatus, descriptionCode);
            }
        }

        public bool IsConnected()
        {
            return _currentStatus == NetworkStatus.Connected;
        }

        public void Disconnect(int disconnectCode)
        {
            ThreadAssert.AssertMainThread();
            ClearPeer(disconnectCode);
            SetStatus(NetworkStatus.Disconnected, disconnectCode);
        }

        private void ClearPeer(int disconnectCode)
        {
            ThreadAssert.AssertMainThread();

            if (_peer != null)
            {
                _peer.Disconnect(disconnectCode);
                _peer.Dispose();
                _peer = null;
            }
        }

        public void SetEncryption(string key)
        {
            _peer?.SetEncryption(key);
        }

        public long Rtt => ((ClientPeer) _peer)?.Rtt ?? 0L;
        
        public bool Send(INetworkOperation operation)
        {
            var result = false;

            ThreadAssert.AssertMainThread();
            operation.Retain();
            if (IsConnected())
            {
                _peer.Send(operation);
                result = true;
            }
            else
            {
                Log.Error("network engine :: can not send operation, operation code = " + operation.OperationCode + ", type = " + operation.Type.GetDescription() + ", is connected = " + IsConnected());
            }
            operation.Release();
            return result;
        }

        public void Connect(IClientContext context, IConnectionConfig config)
        {
            ThreadAssert.AssertMainThread();

            Bro.Log.Info("network engine :: start connection = " + config.Host + ", peer exist = " + (_peer != null));

            SetStatus(NetworkStatus.Connecting);
            
            if (string.IsNullOrEmpty(config.Host))
            {
                SetStatus(NetworkStatus.Disconnected, DisconnectCode.InvalidHost);
                return;
            }

            if (config.Protocol == ConnectionProtocol.Undefined)
            {   
                SetStatus(NetworkStatus.Disconnected, DisconnectCode.InvalidProtocol);
                return;
            }

            ClearPeer(DisconnectCode.Undefined);

            ConnectionConfig = config;

            _peer = CreatePeer(context, config);
            
            if (!_peer.Connect())
            {
                SetStatus(NetworkStatus.Disconnected, DisconnectCode.ConnectionFailed);
            }
        }

        private IClientPeer CreatePeer(IClientContext context, IConnectionConfig config)
        {
            IClientPeer result = null; 
            if (config.Protocol == ConnectionProtocol.Offline)
            {
#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
                result = new OfflineClientPeer(config);
#else
                Bro.Log.Error("Cannot create OfflineClientPeer");
#endif
            }
            else
            {
                result = new ClientPeer(context, config);
            }
            result.OnReceiveLetsEncryptOperation = OnReceiveLetsEncryptOperation;
            result.OnReceiveHandShakeOperation = OnReceiveHandShakeOperation;
            result.OnReceivePingOperation = OnReceivePingOperation;
            result.OnReceiveNetworkEvent = OnReceiveNetworkEvent;
            result.OnReceiveNetworkResponse = OnReceiveNetworkResponse;
            result.OnDisconnect = OnPeerDisconnected;
            result.OnConnect = OnPeerConnected;
            return result;
        }

        private void OnPeerDisconnected(int disconnectCode)
        {
            SetStatus(NetworkStatus.Disconnected, disconnectCode);
        }

        private void OnPeerConnected()
        {
            SetStatus(NetworkStatus.Connected);
        }

        private void OnReceiveNetworkEvent(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();
            operation.Retain();
            try
            {
                OnNetworkEventReceived?.Invoke(operation as INetworkEvent);
            }
            catch (Exception e)
            {
                Bro.Log.Error(e);
            }
            operation.Release();
        }

        private void OnReceiveNetworkResponse(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();
            operation.Retain();
            try
            {
                OnNetworkResponseReceived?.Invoke(operation as INetworkResponse);
            }
            catch (Exception e)
            {
                Bro.Log.Error(e);
            }
            operation.Release();
        }

        private void OnReceiveLetsEncryptOperation(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();
            OnLetsEncryptOperationReceived?.Invoke(operation as LetsEncryptOperation);
        }

        private void OnReceiveHandShakeOperation(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();
            OnHandShakeOperationReceived?.Invoke(operation as HandShakeOperation);
        }

        private void OnReceivePingOperation(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();
            OnPingOperationReceived?.Invoke(operation as PingOperation);
        }
    }
}