using System;
using System.Diagnostics;
using Bro.Encryption;
using Bro.Engine;
using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Client.Network
{
    public class ClientPeer : IClientPeer, IDisposable
    {
        /* statistic */
        long IClientPeer.LastPacketElapsedMs => _receiveTimer.ElapsedMilliseconds;
        public long TotalSent { get; private set; }
        public long TotalReceived { get; private set; }
        /* statistic */

        private readonly Stopwatch _receiveTimer = new Stopwatch();
        private readonly Stopwatch _sendTimer = new Stopwatch();
        private readonly Stopwatch _inactivityTimer = new Stopwatch();

        private readonly BaseSocket _networkPeer;
        private readonly IWriter _dataWriter;
        private readonly IClientContext _context;
        
        private bool _areDefferedOperationsReliable;
        private bool _areDefferedOperationsOrdered;

        private IEncryptor _encryptor;

        private short _currentOperationIndex;

        public Action<INetworkOperation> OnReceiveNetworkEvent { get; set; }
        public Action<INetworkOperation> OnReceiveNetworkResponse { get; set; }
        public Action<INetworkOperation> OnReceiveLetsEncryptOperation { get; set; }
        public Action<INetworkOperation> OnReceivePingOperation { get; set; }
        public Action<INetworkOperation> OnReceiveHandShakeOperation { get; set; }
        Action<int> IClientPeer.OnDisconnect { set => _networkPeer.OnDisconnect = value; }
        Action IClientPeer.OnConnect { set =>  _networkPeer.OnConnect = value; }

        public ClientPeer(IClientContext context, IConnectionConfig config)
        {
            Bro.Log.Info($"client peer :: created, protocol = {config.Protocol}, host = {config.Host}, port = {config.Port}");

            _dataWriter = DataWriter.GetBinaryWriter(NetworkConfig.MessageMaxSize);
            _context = context;
            
            switch (config.Protocol)
            {
                case ConnectionProtocol.Tcp:
                    _networkPeer = new TcpSocket(context, config);
                    break;
                case ConnectionProtocol.Udp:
                    _networkPeer = new UdpSocket(context, config);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _networkPeer.OnDataReceivedBinary = OnDataReceived;
            _dataWriter.Reset();

            _inactivityTimer.Start();
            _receiveTimer.Start();
            _sendTimer.Start();
        }

        public long Rtt => ((UdpSocket) _networkPeer).Rtt;
        public long InactivityTime { get { return _inactivityTimer.ElapsedMilliseconds; } }

        public void Send(INetworkOperation operation)
        {
            //test
            //return;
            if (!_context.IsAlive)
            {
                Bro.Log.Error("client peer :: context is not alive");
                return;
            }
            ThreadAssert.AssertMainThread();

            operation.Retain();

            ++TotalSent;
            _sendTimer.Reset();
            _sendTimer.Start();

            if (_currentOperationIndex > (short.MaxValue - 1))
            {
                // short
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
                if (_encryptor != null)
                {
                    data = _encryptor.Encrypt(data);
                }

                _networkPeer.Send(data, isReliable, isOrdered);

                _dataWriter.Reset();
                _areDefferedOperationsReliable = false;
                _areDefferedOperationsOrdered = false;
            }

            operation.Release();
        }

        private void OnDataReceived(byte[] data)
        {
            //test
            //return;
            ThreadAssert.AssertMainThread();

            ++TotalReceived;
            _receiveTimer.Reset();
            _receiveTimer.Start();

            _inactivityTimer.Reset();
            _inactivityTimer.Start();

            if (_encryptor != null)
            {
                try
                {
                    data = _encryptor.Decrypt(data);
                }
                catch (Exception e)
                {
                    Bro.Log.Error("Error during decryption, it looks like the incoming request is not encrypted, but cliend and server already enstablished encryption " + e);
                    return;
                }
            }

            using (var reader = new BinaryCacheReader(data))
            {
                while (!reader.IsEndOfData)
                {
                    var netOperation = NetworkOperationFactory.Deserialize(reader);
                    if (netOperation == null)
                    {
                        return;
                    }

                    Receive(netOperation);
                }
            }
        }

        private void Receive(INetworkOperation operation)
        {
            ThreadAssert.AssertMainThread();

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
        }

        public void SetEncryption(string key)
        {
            Bro.Log.Info("Client start using encryption key = " + key);
            _encryptor = new AES(key);
        }

        public void Disconnect(int code)
        {
            ThreadAssert.AssertMainThread();
            _networkPeer.Disconnect(code);
        }

        public bool Connect()
        {
            ThreadAssert.AssertMainThread();
            return _networkPeer.Connect();
        }

        public void Dispose()
        {
            ThreadAssert.AssertMainThread();

            if (_networkPeer.State != SocketState.Disconnected)
            {
                Bro.Log.Error("client peer :: was disposed before disconnected");
            }

            OnReceiveNetworkEvent = null;
            OnReceiveNetworkResponse = null;
            OnReceiveLetsEncryptOperation = null;
            OnReceivePingOperation = null;
            OnReceiveHandShakeOperation = null;

            _dataWriter.Dispose();
        }
    }
}