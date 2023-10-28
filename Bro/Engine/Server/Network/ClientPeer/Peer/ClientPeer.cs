using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bro.Encryption;
using Bro.Network;
using Bro.Network.TransmitProtocol;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Server.Observers;
using Bro.Threading;

// ReSharper disable MemberCanBePrivate.Global

namespace Bro.Server
{
    public static class ClientPeerCounter
    {
        private static int _peerCounter;

        public static int GetNext()
        {
            return Interlocked.Increment(ref _peerCounter);
        }
    }

    public class ClientPeer : IClientPeer
    {
        private static int _totalPeers;
        
        private readonly Stopwatch _inactiveStopwatch = new Stopwatch();
        public int PeerId { get; private set; } // autoincrement, current peer index 

        private volatile bool _isDestroying; // whether peer is in the stage of destruction

        public long InactiveTimeMs => _inactiveStopwatch.ElapsedMilliseconds;

        public bool Destroying => _isDestroying;

        public System.IDisposable PeerData { get; set; }

        private int _isSwitchingContext; // is the context switching at the moment
        private int _requestHandlingCounter; // number of requests that are currently processing
        private short _currentOperationIndex; // index of the current operation, to protect against repeat requests

        private IEncryptor _encryptor; // an encryption object, can be null
        private readonly IWriter _dataWriter; // for converting the original message to a byte array
        private IServerContext _activeContext; // the current context in which the peer is located
        private readonly INetworkPeer _networkPeer; // object of low-level peer, different for tcp/udp

        private readonly LetsEncryptObserver _letsEncryptObserver;
        private readonly PingObserver _pingObserver;
        private readonly HandShakeObserver _handShakeObserver;
        private readonly Stats _stats;

        private readonly object _syncSend = new object(); // anchor for sync send


        private bool _areDefferedOperationsReliable;
        private bool _areDefferedOperationsOrdered;
        private readonly List<INetworkRequest> _notHandledRequests = new List<INetworkRequest>();


        public ClientPeer(IServerContext startContext, INetworkPeer peer)
        {
            PeerId = ClientPeerCounter.GetNext();

            Interlocked.Increment(ref _totalPeers);

            Bro.Log.Info("client peer :: created, peer id = " + PeerId);

            _networkPeer = peer;
            _networkPeer.OnConnectHandler = OnConnectHandler;
            _networkPeer.OnReceiveBinaryHandler = Receive;
            _networkPeer.OnDisconnectHandler = OnDisconnectHandler;

            _letsEncryptObserver = new LetsEncryptObserver();
            _pingObserver = new PingObserver();
            _handShakeObserver = new HandShakeObserver(startContext);

            _dataWriter = DataWriter.GetBinaryWriter(Bro.Network.NetworkConfig.MessageMaxSize);
            _stats = new Stats(PeerId);
        }

        public IServerContext Context
        {
            get => _activeContext;
            set
            {
                _activeContext = value;
                if (_activeContext != null)
                {
                    ProcessNotHandledRequests();
                    _inactiveStopwatch.Restart();
                }
            }
        }

        public bool OnStartSwitchingContext()
        {
            var isSwitchingPossible = (_isSwitchingContext == 0);
            Interlocked.Exchange(ref _isSwitchingContext, 1);
            return isSwitchingPossible;
        }

        public void OnFinishSwitchingContext()
        {
            Interlocked.Exchange(ref _isSwitchingContext, 0);
        }

        private void OnConnectHandler()
        {
        }

        private void OnDisconnectHandler(int code)
        {
            Bro.Log.Info("client peer :: disconnected handled code = " + code + ", peed id =" + PeerId);
            
            if (Context == null)
            {
                Dispose();
                return;
            }

            Context.OnDisconnect(this, Dispose);
        }

        public void Disconnect(byte code)
        {
            Bro.Log.Info("client peer :: peer id = " + PeerId + " will be disconnected, code = " + code);
            _networkPeer.Disconnect(code);
        }

        private void Receive(byte[] data)
        {
            if (!_isDestroying)
            {
                NetworkOperationThreadPool.AddOperation(PeerId, () => ProcessReceiveHandler(data));
            }
        }

        private void OnReceive(INetworkRequest request)
        {
            _stats.OnReceiveOperation(request);
            _stats.Log();

            if (Context != null)
            {
                Context.HandleRequest(request, this, null);
            }
            else
            {
                _notHandledRequests.Add(request);
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

        private void ProcessReceiveHandler(byte[] data)
        {
            var point = PerformanceMeter.Register(PerformancePointType.ClientPeerReceiveData);

            _inactiveStopwatch.Restart();

            if (!_isDestroying)
            {
                try
                {
                    if (_encryptor != null)
                    {
                        data = _encryptor.Decrypt(data);
                    }
                }
                catch (Exception ex)
                {
                    Bro.Log.Info("client peer :: can not decrypt received data, " + ex.Message);
                    point?.Done();
                    return;
                }

                try
                {
                    using (var reader = DataReader.GetBinaryReader(data))
                    {
                        while (!reader.IsEndOfData)
                        {
                            var netOperation = NetworkOperationFactory.Deserialize(reader);
                            Receive(netOperation);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Bro.Log.Info("client peer :: can not parse incoming request, data = " + data);
                    Log.Error(ex.StackTrace);
                }
            }

            point?.Done();
        }

        private void Receive(INetworkOperation operation)
        {
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

        private void ProcessNotHandledRequests()
        {
            foreach (var request in _notHandledRequests)
            {
                Context.HandleRequest(request, this, null);
            }

            _notHandledRequests.Clear();
        }

        void IClientPeer.SetEncryption(string key)
        {
            Bro.Log.Info("Peer peerId = " + PeerId + " start using encryption, key = " + key);
            _encryptor = new AES(key);
        }
        
        void IClientPeer.Send(INetworkOperation operation)
        {
            _inactiveStopwatch.Restart();
            operation.Retain();
            if (!_isDestroying)
            {
                NetworkOperationThreadPool.AddOperation(PeerId, () =>
                {
                    ProcessSend(operation);
                    operation.Release();
                });
            }
            else
            {
                NetworkOperationThreadPool.AddOperation(PeerId, () => operation.Release());
            }
        }


        private void ProcessSend(INetworkOperation operation)
        {
            var point = PerformanceMeter.Register(PerformancePointType.ClientPeerSendData);
            
            lock (_syncSend)
            {
                _stats.OnSendOperation(operation);

                if (!_isDestroying)
                {
                    if (_currentOperationIndex >= (short.MaxValue))
                    {
                        _currentOperationIndex = 0;
                    }

                    operation.OperationCounter = ++_currentOperationIndex;

                    NetworkOperationFactory.Serialize(_dataWriter, operation);

                    if (operation.IsDeferred)
                    {
                        _areDefferedOperationsReliable = _areDefferedOperationsReliable || operation.IsReliable;
                        _areDefferedOperationsOrdered = _areDefferedOperationsOrdered || operation.IsOrdered;
                    }
                    else
                    {
                        var isReliable = _areDefferedOperationsReliable || operation.IsReliable;
                        var isOrdered = operation.IsOrdered || _areDefferedOperationsOrdered;

                        var data = _dataWriter.Data;
                        if (data != null)
                        {
                            if (_encryptor != null)
                            {
                                data = _encryptor.Encrypt(data);
                            }

                            _stats.OnSendBytes(data.Length);

                            NetworkOperationThreadPool.AddOperation(_networkPeer.Id, () => _networkPeer.Send(data, isReliable, isOrdered));
                        }

                        _dataWriter.Reset();
                        _areDefferedOperationsReliable = false;
                        _areDefferedOperationsOrdered = false;
                    }
                }
            }

            point?.Done();
        }

        public void OnStartHandleRequest()
        {
            Interlocked.Increment(ref _requestHandlingCounter);
        }

        public void OnEndHandleRequest()
        {
            Interlocked.Decrement(ref _requestHandlingCounter);

            if (_isDestroying && _requestHandlingCounter == 0)
            {
                CompleteDispose();
            }
        }

        private void Dispose()
        {
            if (!_isDestroying)
            {
                _isDestroying = true;
                Interlocked.Decrement(ref _totalPeers);
            }

            if (_requestHandlingCounter == 0)
            {
                CompleteDispose();
            }
        }

        private void ClearPeerData()
        {
            var peerData = PeerData;
            if (peerData != null)
            {
                peerData.Dispose();
                PeerData = null;
            }
        }

        private void CompleteDispose()
        {
            Bro.Log.Info("client peer :: complete dispose called, peed id = " + PeerId + ", context = " + Context);
            
            var context = Context;
            if (context != null)
            {
                context.Scheduler.Schedule(ClearPeerData);
            }
            else
            {
                ClearPeerData();
            }

            lock (_syncSend)
            {
                _dataWriter.Dispose();
            }
        }
    }
}