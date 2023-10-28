using System;
using System.Diagnostics;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Observers;

namespace Bro.Server.Network
{
    public class OfflineClientPeer : IClientPeer
    {
        private readonly Stopwatch _inactiveStopwatch = new Stopwatch();
        public int PeerId { get; private set; }
        public IServerContext Context { get; set; }
        public System.IDisposable PeerData { get; set; }
        
        public long InactiveTimeMs => _inactiveStopwatch.ElapsedMilliseconds;

        public bool Destroying => false;

        private readonly INetworkPeer _networkPeer;
        private readonly LetsEncryptObserver _letsEncryptObserver;
        private readonly PingObserver _pingObserver;
        private readonly HandShakeObserver _handShakeObserver;

        public OfflineClientPeer(IServerContext startContext, INetworkPeer peer)
        {
            _inactiveStopwatch.Start();
            PeerId = ClientPeerCounter.GetNext();

            Bro.Log.Info("offline :: client peer created, peerId = " + PeerId);

            _letsEncryptObserver = new LetsEncryptObserver();
            _pingObserver = new PingObserver();
            _handShakeObserver = new HandShakeObserver(startContext);

            _networkPeer = peer;
            _networkPeer.OnDisconnectHandler = OnDisconnectHandler;
        }

        public void Disconnect(byte code)
        {
            _networkPeer.Disconnect(code);
        }

        private void OnDisconnectHandler(int code)
        {
            Context?.OnDisconnect(this, () => PeerData?.Dispose());
        }

        public void Send(INetworkOperation operation)
        {
            _inactiveStopwatch.Restart();

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            OfflineBridge.ServerSendOperation(operation);
#endif
        }

        public void Receive(INetworkOperation operation)
        {
            _inactiveStopwatch.Restart();

            if (operation != null)
            {
                switch (operation.Type)
                {
                    case NetworkOperationType.Request:
                        OnReceive(operation as INetworkRequest);
                        break;
                    case NetworkOperationType.Response:
                        Bro.Log.Error("Received response operation, server not handle responses!");
                        break;
                    case NetworkOperationType.Event:
                        Bro.Log.Error("Received event operation, server not handle events!");
                        break;
                    case NetworkOperationType.Ping:
                        OnReceive(operation as PingOperation);
                        break;
                    case NetworkOperationType.Encryption:
                        OnReceive(operation as LetsEncryptOperation);
                        break;
                    case NetworkOperationType.Handshake:
                        OnReceive(operation as HandShakeOperation);
                        break;
                    default:
                        throw new System.Exception("Cannot handle with " + operation.Type);
                }
            }
        }

        private void OnReceive(HandShakeOperation handShakeOperation)
        {
            _handShakeObserver.OnReceive(handShakeOperation, this);
        }

        private void OnReceive(PingOperation pingOperation)
        {
            _pingObserver.OnReceive(pingOperation, this);
        }

        private void OnReceive(LetsEncryptOperation letsEncryptOperation)
        {
            _letsEncryptObserver.OnReceive(letsEncryptOperation, this);
        }

        private void OnReceive(INetworkRequest request)
        {
            if (Context != null)
            {
                request.Retain();
                Context.HandleRequest(request, this, request.Release);
            }
        }

        public void SetEncryption(string key)
        {
            Bro.Log.Info("Server OfflinePeer received encryption key = " + key);
        }

        public bool OnStartSwitchingContext()
        {
            return true;
        }

        public void OnFinishSwitchingContext()
        {
        }

        public void OnStartHandleRequest()
        {
        }

        public void OnEndHandleRequest()
        {
        }
    }
}