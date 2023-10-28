// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bro.Threading;

namespace Bro.Network.Udp.Engine
{
    internal sealed class EndPointComparer : IEqualityComparer<IPEndPoint>
    {
        public bool Equals(IPEndPoint x, IPEndPoint y)
        {
            return y != null && (x != null && (x.Address.Equals(y.Address) && x.Port == y.Port));
        }

        public int GetHashCode(IPEndPoint obj)
        {
            return obj.GetHashCode();
        }
    }
    
    internal sealed class NetEvent
    {
        public enum EType
        {
            Connect,
            Disconnect,
            Receive,
            ConnectionRequest,
        }
        
        public EType Type;
        public NetPeer Peer;
        public byte[] Data;
        public int DisconnectionCode;
        
        public ConnectionRequest ConnectionRequest;
    }

    public class NetManager : INetSocketListener
    {
        private readonly NetSocket _socket;
        private BroThread _logicThread;

        private readonly Queue<NetEvent> _netEventsQueue;
        private readonly Stack<NetEvent> _netEventsPool;
        private readonly INetEventListener _netEventListener;
      
        private readonly Dictionary<IPEndPoint, NetPeer> _peersDict;
        private readonly Dictionary<IPEndPoint, ConnectionRequest> _requestsDict;
        private readonly ReaderWriterLockSlim _peersLock;
        private volatile NetPeer _headPeer;
        private NetPeer[] _peersArray;
        private int _lastPeerId;
        private readonly Queue<int> _peerIds;
        
        internal readonly NetPacketPool NetPacketPool;

        private const int UpdateTime = 33;
        public const int PingInterval = 1000;
        public const int DisconnectTimeout = 5000;
        public const int ReconnectDelay = 500;
        public const int MaxConnectAttempts = 5;
        
        public NetManager(INetEventListener listener)
        {
            _socket = new NetSocket(this);
            _netEventListener = listener;
            _netEventsQueue = new Queue<NetEvent>();
            _netEventsPool = new Stack<NetEvent>();
            NetPacketPool = new NetPacketPool();
            _peersDict = new Dictionary<IPEndPoint, NetPeer>(new EndPointComparer());
            _requestsDict = new Dictionary<IPEndPoint, ConnectionRequest>(new EndPointComparer());
            _peersLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _peerIds = new Queue<int>();
            _peersArray = new NetPeer[32];
        }
        
        private bool TryGetPeer(IPEndPoint endPoint, out NetPeer peer)
        {
            _peersLock.EnterReadLock();
            var result = _peersDict.TryGetValue(endPoint, out peer);
            _peersLock.ExitReadLock();
            return result;
        }

        private void AddPeer(NetPeer peer)
        {
            _peersLock.EnterWriteLock();
            if (_headPeer != null)
            {
                peer.NextPeer = _headPeer;
                _headPeer.PrevPeer = peer;
            }
            _headPeer = peer;
            _peersDict.Add(peer.EndPoint, peer);
            if (peer.Id >= _peersArray.Length)
            {
                var newSize = _peersArray.Length * 2;
                while (peer.Id >= newSize)
                {
                    newSize *= 2;
                }
                Array.Resize(ref _peersArray, newSize);
            }
            _peersArray[peer.Id] = peer;
            SystemMonitoring.NetworkPeersCount?.Inc();
            SystemMonitoring.NetworkConnectionsRate?.Observe(1);

            _peersLock.ExitWriteLock();
        }

        private void RemovePeer(NetPeer peer)
        {
            _peersLock.EnterWriteLock();
            RemovePeerInternal(peer);
            _peersLock.ExitWriteLock();
        }

        private void RemovePeerInternal(NetPeer peer)
        {
            if (!_peersDict.Remove(peer.EndPoint))
            {
                return;
            }

            if (peer == _headPeer)
            {
                _headPeer = peer.NextPeer;
            }

            if (peer.PrevPeer != null)
            {
                peer.PrevPeer.NextPeer = peer.NextPeer;
            }

            if (peer.NextPeer != null)
            {
                peer.NextPeer.PrevPeer = peer.PrevPeer;
            }
            peer.PrevPeer = null;

            _peersArray[peer.Id] = null;
            SystemMonitoring.NetworkPeersCount?.Dec();
            SystemMonitoring.NetworkDisconnectionsRate?.Observe(1);
            
            lock (_peerIds)
            {
                _peerIds.Enqueue(peer.Id);
            }
        }
  
        internal int SendRawAndRecycle(NetPacket packet, IPEndPoint remoteEndPoint)
        {
            var result = SendRaw(packet.RawData, 0, packet.Size, remoteEndPoint);
            NetPacketPool.Recycle(packet);
            return result;
        }

        internal int SendRaw(NetPacket packet, IPEndPoint remoteEndPoint)
        {
            return SendRaw(packet.RawData, 0, packet.Size, remoteEndPoint);
        }

        internal int SendRaw(byte[] message, int start, int length, IPEndPoint remoteEndPoint)
        {
            if (!_socket.IsRunning)
            {
                return 0;
            }

            SocketError errorCode = 0;
            
            var result = _socket.SendTo(message, start, length, remoteEndPoint, ref errorCode);
            
            NetPeer fromPeer;
            
            switch (errorCode)
            {
                case SocketError.MessageSize:
                    return -1;
                case SocketError.HostUnreachable:
                    if (TryGetPeer(remoteEndPoint, out fromPeer))
                    {
                        DisconnectPeer(fromPeer, DisconnectCode.HostUnreachable, true );
                    }
                    return -1;
                case SocketError.NetworkUnreachable:
                    if (TryGetPeer(remoteEndPoint, out fromPeer))
                    {
                        DisconnectPeer(fromPeer, DisconnectCode.NetworkUnreachable, true );
                    }
                    return -1;
            }

            return result <= 0 ? 0 : result;
        }

        public void DisconnectPeer( NetPeer peer, int code, bool force )
        {
            var shutdownResult = peer.Shutdown( code, force );
            if (shutdownResult == ShutdownResult.None)
            {
                return;
            }
            CreateEvent( NetEvent.EType.Disconnect, peer, null, null, code );
        }

        private void CreateEvent( NetEvent.EType type, NetPeer peer = null, ConnectionRequest connectionRequest = null, NetPacket packet = null, int disconnectionCode = 0)
        {
            NetEvent evt;
            
            lock (_netEventsPool)
            {
                evt = _netEventsPool.Count > 0 ? _netEventsPool.Pop() : new NetEvent();
            }
            
            evt.Type = type;
            evt.Data = packet?.GetPacketData();
            evt.Peer = peer;
            evt.DisconnectionCode = disconnectionCode;
            evt.ConnectionRequest = connectionRequest;
            
            NetPacketPool.Recycle(packet);
            
            lock (_netEventsQueue)
            {
                _netEventsQueue.Enqueue(evt);
            }
        }

        private void ProcessEvent(NetEvent evt)
        {
            switch (evt.Type)
            {
                case NetEvent.EType.Connect:
                    _netEventListener.OnPeerConnected(evt.Peer);
                    break;
                case NetEvent.EType.Disconnect:
                    _netEventListener.OnPeerDisconnected(evt.Peer, evt.DisconnectionCode);
                    break;
                case NetEvent.EType.Receive:
                    _netEventListener.OnNetworkReceive(evt.Peer, evt.Data);
                    break;
                case NetEvent.EType.ConnectionRequest:
                    _netEventListener.OnConnectionRequest(evt.ConnectionRequest);
                    break;
            }
       
            RecycleEvent(evt);
        }

        private void RecycleEvent(NetEvent evt)
        {
            evt.Peer = null;
            evt.ConnectionRequest = null;
            evt.Data = null;
            evt.DisconnectionCode = 0;
            
            lock (_netEventsPool)
            {
                _netEventsPool.Push(evt);
            }
        }

        private void UpdateLogic()
        {
            var peersToRemove = new List<NetPeer>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (_socket.IsRunning)
            {
                var elapsed = (int)stopwatch.ElapsedMilliseconds;
                elapsed = elapsed <= 0 ? 1 : elapsed;
                stopwatch.Reset();
                stopwatch.Start();

                for (var netPeer = _headPeer; netPeer != null; netPeer = netPeer.NextPeer)
                {
                    if (netPeer.ConnectionState == ConnectionState.Disconnected && netPeer.TimeSinceLastPacket > DisconnectTimeout)
                    {
                        peersToRemove.Add(netPeer);
                    }
                    else
                    {
                        netPeer.Update(elapsed);
                    }
                }
                if (peersToRemove.Count > 0)
                {
                    _peersLock.EnterWriteLock();
                    for (int i = 0; i < peersToRemove.Count; i++)
                    {
                        RemovePeerInternal(peersToRemove[i]);
                    }
                    _peersLock.ExitWriteLock();
                    peersToRemove.Clear();
                }

                var sleepTime = UpdateTime - (int)stopwatch.ElapsedMilliseconds;
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
            stopwatch.Stop();
        }
        
        void INetSocketListener.OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint)
        {
            SystemMonitoring.NetworkUdpReceivedBytes?.Observe(length);
            
            if (errorCode != 0)
            {
                return;
            }

            try
            {
                DataReceived(data, length, remoteEndPoint);
            }
            catch(Exception e)
            {
                Bro.Log.Error(e);
            }
        }

        internal NetPeer OnConnectionSolved(ConnectionRequest request)
        {
            NetPeer netPeer = null;

            if (request.Result == ConnectionRequestResult.RejectForce)
            {
                var shutdownPacket = NetPacketPool.GetWithProperty(PacketProperty.Disconnect, 4);
                shutdownPacket.ConnectionNumber = request.ConnectionNumber;
                FastBitConverter.GetBytes(shutdownPacket.RawData, 1, request.ConnectionTime);
                Buffer.BlockCopy(BitConverter.GetBytes(0), 0, shutdownPacket.RawData, 9, 4);
                SendRawAndRecycle(shutdownPacket, request.RemoteEndPoint);
            }
            else
            {
                _peersLock.EnterUpgradeableReadLock();
                if (_peersDict.TryGetValue(request.RemoteEndPoint, out netPeer))
                {
                    _peersLock.ExitUpgradeableReadLock();
                }
                else if (request.Result == ConnectionRequestResult.Reject)
                {
                    netPeer = new NetPeer(this, request.RemoteEndPoint, GetNextPeerId());
                    netPeer.Reject(request.ConnectionTime, request.ConnectionNumber);
                    AddPeer(netPeer);
                    _peersLock.ExitUpgradeableReadLock();
                }
                else
                {
                    netPeer = new NetPeer(this, request.RemoteEndPoint, GetNextPeerId(), request.ConnectionTime, request.ConnectionNumber);
                    AddPeer(netPeer);
                    _peersLock.ExitUpgradeableReadLock();
                    CreateEvent(NetEvent.EType.Connect, netPeer);
                }
            }

            lock (_requestsDict)
            {
                _requestsDict.Remove(request.RemoteEndPoint);
            }

            return netPeer;
        }

        private int GetNextPeerId()
        {
            lock (_peerIds)
            {
                return _peerIds.Count == 0 ? _lastPeerId++ : _peerIds.Dequeue();
            }
        }

        private void ProcessConnectRequest( IPEndPoint remoteEndPoint, NetPeer netPeer, NetConnectRequestPacket connRequest)
        {
            var connectionNumber = connRequest.ConnectionNumber;
            ConnectionRequest req;
            
            if (netPeer != null)
            {
                var processResult = netPeer.ProcessConnectRequest(connRequest);
                switch (processResult)
                {
                    case ConnectRequestResult.Reconnection:
                        DisconnectPeer(netPeer, DisconnectCode.ConnectionRejected, true);
                        RemovePeer(netPeer);
                        break;
                    case ConnectRequestResult.NewConnection:
                        RemovePeer(netPeer);
                        break;
                    case ConnectRequestResult.P2PConnection:
                        lock (_requestsDict)
                        {
                            req = new ConnectionRequest( netPeer.ConnectTime, connectionNumber, ConnectionRequestType.PeerToPeer, remoteEndPoint, this);
                            _requestsDict.Add(remoteEndPoint, req);
                        }
                        CreateEvent(NetEvent.EType.ConnectionRequest, connectionRequest: req);
                        return;
                    default:
                        return;
                }
                connectionNumber = (byte)((netPeer.ConnectionNum + 1) % NetConstants.MaxConnectionNumber);
            }

            lock (_requestsDict)
            {
                if (_requestsDict.TryGetValue(remoteEndPoint, out req))
                {
                    req.UpdateRequest(connRequest);
                    return;
                }
                req = new ConnectionRequest(connRequest.ConnectionTime,connectionNumber,ConnectionRequestType.Incoming,remoteEndPoint,this);
                _requestsDict.Add(remoteEndPoint, req);
            }
            CreateEvent(NetEvent.EType.ConnectionRequest, connectionRequest: req);
        }

        private void DataReceived(byte[] reusableBuffer, int count, IPEndPoint remoteEndPoint)
        {
            var packet = NetPacketPool.GetPacket(count);
            if (!packet.FromBytes(reusableBuffer, 0, count))
            {
                NetPacketPool.Recycle(packet);
                return;
            }

            NetPeer netPeer;
            if (packet.Property == PacketProperty.ConnectRequest)
            {
                if (NetConnectRequestPacket.GetProtocolId(packet) != NetConstants.ProtocolId)
                {
                    SendRawAndRecycle(NetPacketPool.GetWithProperty(PacketProperty.InvalidProtocol), remoteEndPoint);
                    return;
                }
                var connRequest = NetConnectRequestPacket.FromData(packet);
                if (connRequest != null)
                {
                    _peersLock.EnterUpgradeableReadLock();
                    _peersDict.TryGetValue(remoteEndPoint, out netPeer);
                    ProcessConnectRequest(remoteEndPoint, netPeer, connRequest);
                    _peersLock.ExitUpgradeableReadLock();
                }
                return;
            }

            _peersLock.EnterReadLock();
            var peerFound = _peersDict.TryGetValue(remoteEndPoint, out netPeer);
            _peersLock.ExitReadLock();
            
            switch (packet.Property)
            {
                case PacketProperty.PeerNotFound:
                    if (peerFound)
                    {
                        if (netPeer.ConnectionState != ConnectionState.Connected)
                        {
                            return;
                        }
                        if (packet.Size == 1) 
                        {
                            var p = NetPacketPool.GetWithProperty(PacketProperty.PeerNotFound, 9);
                            p.RawData[1] = 0;
                            FastBitConverter.GetBytes(p.RawData, 2, netPeer.ConnectTime);
                            SendRawAndRecycle(p, remoteEndPoint);
                        }
                        else if (packet.Size == 10 && packet.RawData[1] == 1 && BitConverter.ToInt64(packet.RawData, 2) == netPeer.ConnectTime) 
                        {
                            DisconnectPeer(netPeer, DisconnectCode.RemoteConnectionClose, true);
                        }
                    }
                    else if (packet.Size == 10 && packet.RawData[1] == 0)
                    {
                        packet.RawData[1] = 1;
                        SendRawAndRecycle(packet, remoteEndPoint);
                    }
                    break;
                case PacketProperty.InvalidProtocol:
                    if (peerFound && netPeer.ConnectionState == ConnectionState.Outgoing)
                    {
                        DisconnectPeer(netPeer, DisconnectCode.InvalidProtocol, true);
                    }
                    break;
                case PacketProperty.Disconnect:
                    if (peerFound)
                    {
                        var disconnectResult = netPeer.ProcessDisconnect(packet);
                        if (disconnectResult == DisconnectResult.None)
                        {
                            NetPacketPool.Recycle(packet);
                            return;
                        }
                    
                        var code = packet.GetIntegerCode();
                        DisconnectPeer( netPeer, code, true );
                    }
                    else
                    {
                        NetPacketPool.Recycle(packet);
                    }

                    SendRawAndRecycle(NetPacketPool.GetWithProperty(PacketProperty.ShutdownOk), remoteEndPoint);
                    break;

                case PacketProperty.ConnectAccept:
                    var connAccept = NetConnectAcceptPacket.FromData(packet);
                    if (connAccept != null && peerFound && netPeer.ProcessConnectAccept(connAccept))
                    {
                        CreateEvent(NetEvent.EType.Connect, netPeer);
                    }
                    break;
                default:
                    if (peerFound)
                    {
                        netPeer.ProcessPacket(packet);
                    }
                    else
                    {
                        SendRawAndRecycle(NetPacketPool.GetWithProperty(PacketProperty.PeerNotFound), remoteEndPoint);
                    }
                    break;
            }
        }

        internal void CreateReceiveEvent(NetPacket packet, NetPeer fromPeer)
        {
            NetEvent evt;
            lock (_netEventsPool)
            {
                evt = _netEventsPool.Count > 0 ? _netEventsPool.Pop() : new NetEvent();
            }
            evt.Type = NetEvent.EType.Receive;
            evt.Data = packet.GetPacketData(); 
            evt.Peer = fromPeer;
            
            NetPacketPool.Recycle(packet);
            
            lock (_netEventsQueue)
            {
                _netEventsQueue.Enqueue(evt);
            }
        }
        
        public bool Start(IPAddress addressIPv4, IPAddress addressIPv6, int port)
        {
            if (!_socket.Bind(addressIPv4, addressIPv6, port))
            {
                return false;
            }
            _logicThread = new BroThread(UpdateLogic) { Name = "LogicThread", IsBackground = true };
            _logicThread.Start();
            return true;
        }
        
        public bool Start(int port = 0)
        {
            return Start(IPAddress.Any, IPAddress.IPv6Any, port);
        }

        public void PollEvents()
        {
            while (true)
            {
                NetEvent evt;
                lock (_netEventsQueue)
                {
                    if (_netEventsQueue.Count > 0)
                        evt = _netEventsQueue.Dequeue();
                    else
                        return;
                }
                ProcessEvent(evt);
            }
        }
        
        public NetPeer Connect(string address, int port)
        {
            IPEndPoint endPoint;
            try
            {
                endPoint = NetUtils.MakeEndPoint(address, port);
            }
            catch
            {
                return null;
            }
            return Connect(endPoint);
        }

        private NetPeer Connect(IPEndPoint target)
        {
            NetPeer peer;
            byte connectionNumber = 0;

            lock (_requestsDict)
            {
                if (_requestsDict.ContainsKey(target))
                {
                    return null;
                }
            }

            _peersLock.EnterUpgradeableReadLock();
            if (_peersDict.TryGetValue(target, out peer))
            {
                switch (peer.ConnectionState)
                {
                    case ConnectionState.Connected:
                    case ConnectionState.Outgoing:
                        _peersLock.ExitUpgradeableReadLock();
                        return peer;
                }

                connectionNumber = (byte)((peer.ConnectionNum + 1) % NetConstants.MaxConnectionNumber);
                RemovePeer(peer);
            }

            peer = new NetPeer(this, target, GetNextPeerId(), connectionNumber);
            AddPeer(peer);
            _peersLock.ExitUpgradeableReadLock();

            return peer;
        }

        public void Stop()
        {
            Stop(true);
        }

        public void Stop(bool sendDisconnectMessages)
        {
            if (!_socket.IsRunning)
            {
                return;
            }
            
            for (var netPeer = _headPeer; netPeer != null; netPeer = netPeer.NextPeer)
            {
                netPeer.Shutdown( DisconnectCode.System, ! sendDisconnectMessages );
            }

            _socket.Close(false);
            _logicThread.Join();
            _logicThread = null;
            
            _peersLock.EnterWriteLock();
            _headPeer = null;
            _peersDict.Clear();
            _peersArray = new NetPeer[32];
            _peersLock.ExitWriteLock();
            lock (_peerIds)
            {
                _peerIds.Clear();
            }
            
            lock (_netEventsQueue)
            {
                _netEventsQueue.Clear();
            }
        }

        public void GetPeersNonAlloc(List<NetPeer> peers, ConnectionState peerState)
        {
            peers.Clear();
            _peersLock.EnterReadLock();
            for (var netPeer = _headPeer; netPeer != null; netPeer = netPeer.NextPeer)
            {
                if ((netPeer.ConnectionState & peerState) != 0)
                    peers.Add(netPeer);
            }
            _peersLock.ExitReadLock();
        }

        public int GetPeersCount(ConnectionState peerState)
        {
            var count = 0;
            _peersLock.EnterReadLock();
            for (var netPeer = _headPeer; netPeer != null; netPeer = netPeer.NextPeer)
            {
                if ((netPeer.ConnectionState & peerState) != 0)
                {
                    count++;
                }
            }
            _peersLock.ExitReadLock();
            return count;
        }
    }
}