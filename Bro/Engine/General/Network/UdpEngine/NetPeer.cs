// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Bro.Network.Udp.Engine
{
    [Flags]
    public enum ConnectionState : byte
    {
        Outgoing         = 1 << 1,
        Connected         = 1 << 2,
        ShutdownRequested = 1 << 3,
        Disconnected      = 1 << 4,
        Any = Outgoing | Connected | ShutdownRequested
    }

    internal enum ConnectRequestResult
    {
        None,
        P2PConnection, //when peer connecting
        Reconnection,  //when peer was connected
        NewConnection  //when peer was disconnected
    }

    internal enum DisconnectResult
    {
        None,
        Reject,
        Disconnect
    }

    internal enum ShutdownResult
    {
        None,
        Success,
        WasConnected
    }

    public class NetPeer : ITransportPeer
    {
        public object NetworkPeer { get; set; }
        
        public long Rtt => _avgRtt;
        
        private int _rtt;
        private int _avgRtt;
        private int _rttCount;
        private double _resendDelay = 27.0;
        private int _pingSendTimer;
        private int _rttResetTimer;
        private readonly Stopwatch _pingTimer = new Stopwatch();
        private int _timeSinceLastPacket;
        
        private readonly NetPacketPool _packetPool;
        private readonly object _flushLock = new object();
        private readonly object _sendLock = new object();
        private readonly object _shutdownLock = new object();

        internal volatile NetPeer NextPeer;
        internal NetPeer PrevPeer;

        internal byte ConnectionNum
        {
            get { return _connectNum; }
            private set
            {
                _connectNum = value;
                _mergeData.ConnectionNumber = value;
                _pingPacket.ConnectionNumber = value;
                _pongPacket.ConnectionNumber = value;
            }
        }
 
        //Channels
        private readonly Queue<NetPacket> _unreliableChannel;
        private readonly BaseChannel[] _channels;
        private BaseChannel _headChannel;

        //MTU
        private int _mtu;
        private int _mtuIdx;
        private bool _finishMtu;
        private int _mtuCheckTimer;
        private int _mtuCheckAttempts;
        private const int MtuCheckDelay = 1000;
        private const int MaxMtuCheckAttempts = 4;
        private readonly object _mtuMutex = new object();

        //Fragment
        private class IncomingFragments
        {
            public NetPacket[] Fragments;
            public int ReceivedCount;
            public int TotalSize;
            public byte ChannelId;
        }
        private ushort _fragmentId;
        private readonly Dictionary<ushort, IncomingFragments> _holdedFragments;
        
        //Merging
        private readonly NetPacket _mergeData;
        private int _mergePos;
        private int _mergeCount;

        //Connection
        private int _connectAttempts;
        private int _connectTimer;
        private long _connectTime;
        private byte _connectNum;
        private ConnectionState _connectionState;
        private NetPacket _shutdownPacket;
        private const int ShutdownDelay = 300;
        private int _shutdownTimer;
        private readonly NetPacket _pingPacket;
        private readonly NetPacket _pongPacket;
        private readonly NetPacket _connectRequestPacket;
        private readonly NetPacket _connectAcceptPacket;

        public readonly int Id;
        
        public readonly IPEndPoint EndPoint;

        public readonly NetManager NetManager;

        public ConnectionState ConnectionState { get { return _connectionState; } }
        
        internal long ConnectTime { get { return _connectTime; } }
        
        public int TimeSinceLastPacket { get { return _timeSinceLastPacket; } }

        internal double ResendDelay { get { return _resendDelay; } }
        
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id)
        {
            Id = id;
            _packetPool = netManager.NetPacketPool;
            NetManager = netManager;
            SetMtu(0);

            EndPoint = remoteEndPoint;
            _connectionState = ConnectionState.Connected;
            _mergeData = new NetPacket(PacketProperty.Merged, NetConstants.MaxPacketSize);
            _pongPacket = new NetPacket(PacketProperty.Pong, 0);
            _pingPacket = new NetPacket(PacketProperty.Ping, 0) {Sequence = 1};
           
            _unreliableChannel = new Queue<NetPacket>(64);
            _headChannel = null;
            _holdedFragments = new Dictionary<ushort, IncomingFragments>();
            
            _channels = new BaseChannel[4];
        }

        private void SetMtu(int mtuIdx)
        {
            _mtu = NetConstants.PossibleMtu[mtuIdx];
        }

        private BaseChannel CreateChannel(byte idx)
        {
            var newChannel = _channels[idx];
            if (newChannel != null)
            {
                return newChannel;
            }
            
            switch ((DeliveryMethod)(idx % 4))
            {
                case DeliveryMethod.ReliableUnordered:
                    newChannel = new ReliableChannel(this, false, idx);
                    break;
                case DeliveryMethod.Sequenced:
                    newChannel = new SequencedChannel(this, false, idx);
                    break;
                case DeliveryMethod.ReliableOrdered:
                    newChannel = new ReliableChannel(this, true, idx);
                    break;
                case DeliveryMethod.ReliableSequenced:
                    newChannel = new SequencedChannel(this, true, idx);
                    break;
            }
            _channels[idx] = newChannel;
            
            if (newChannel != null)
            {
                newChannel.Next = _headChannel;
                _headChannel = newChannel;
                return newChannel;
            }

            return null;
        }
        
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, byte connectNum) : this(netManager, remoteEndPoint, id)
        {
            _connectTime = DateTime.UtcNow.Ticks;
            _connectionState = ConnectionState.Outgoing;
            ConnectionNum = connectNum;

            _connectRequestPacket = NetConnectRequestPacket.Make(_connectTime);
            _connectRequestPacket.ConnectionNumber = connectNum;

            NetManager.SendRaw(_connectRequestPacket, EndPoint);
        }
        
        internal NetPeer(NetManager netManager, IPEndPoint remoteEndPoint, int id, long connectId, byte connectNum) : this(netManager, remoteEndPoint, id)
        {
            _connectTime = connectId;
            _connectionState = ConnectionState.Connected;
            ConnectionNum = connectNum;

            _connectAcceptPacket = NetConnectAcceptPacket.Make(_connectTime, connectNum, false);
            
            NetManager.SendRaw(_connectAcceptPacket, EndPoint);
        }

        internal void Reject(long connectionId, byte connectionNumber)
        {
            _connectTime = connectionId;
            _connectNum = connectionNumber;
            Shutdown( DisconnectCode.ConnectionRejected, false);
        }

        internal bool ProcessConnectAccept(NetConnectAcceptPacket packet)
        {
            if (_connectionState != ConnectionState.Outgoing)
            {
                return false;
            }

            if (packet.ConnectionId != _connectTime)
            {
                return false;
            }
   
            ConnectionNum = packet.ConnectionNumber;

            Interlocked.Exchange(ref _timeSinceLastPacket, 0);
            _connectionState = ConnectionState.Connected;
            return true;
        }

        public void Send(byte[] data, DeliveryMethod deliveryMethod)
        {
            SendInternal(data, 0, data.Length, deliveryMethod);
        }
        
        private void SendInternal( byte[] data,  int start,  int length, DeliveryMethod deliveryMethod)
        {
            var channelNumber = 0;
            if (_connectionState != ConnectionState.Connected)
            {
                return;
            }

            PacketProperty property;
            BaseChannel channel = null;

            if (deliveryMethod == DeliveryMethod.Unreliable)
            {
                property = PacketProperty.Unreliable;
            }
            else
            {
                property = PacketProperty.Channeled;
                channel = CreateChannel((byte)(channelNumber*4 + (byte)deliveryMethod));
            }
            
            var headerSize = NetPacket.GetHeaderSize(property);
            var mtu = _mtu;
            if (length + headerSize > mtu)
            {
                if (deliveryMethod != DeliveryMethod.ReliableOrdered && deliveryMethod != DeliveryMethod.ReliableUnordered)
                {
                    Bro.Log.Error("net peer :: unreliable packet size exceeded maximum of " + (mtu - headerSize) + " bytes");                    
                }

                var packetFullSize = mtu - headerSize;
                var packetDataSize = packetFullSize - NetConstants.FragmentHeaderSize;
                var totalPackets = length / packetDataSize + (length % packetDataSize == 0 ? 0 : 1);
                
                if (totalPackets > ushort.MaxValue)
                {
                    Bro.Log.Error("net peer :: data was split in " + totalPackets + " fragments, which exceeds " + ushort.MaxValue);                    
                }
                
                ushort currentFragmentId;
                lock (_sendLock)
                {
                    currentFragmentId = _fragmentId;
                    _fragmentId++;
                }

                for(ushort partIdx = 0; partIdx < totalPackets; partIdx++)
                {
                    var sendLength = length > packetDataSize ? packetDataSize : length;
                    var p = _packetPool.GetPacket(headerSize + sendLength + NetConstants.FragmentHeaderSize);
                    p.Property = property;
                    p.FragmentId = currentFragmentId;
                    p.FragmentPart = partIdx;
                    p.FragmentsTotal = (ushort)totalPackets;
                    p.MarkFragmented();

                    Buffer.BlockCopy(data, partIdx * packetDataSize, p.RawData, NetConstants.FragmentedHeaderTotalSize, sendLength);
                    if (channel != null)
                    {
                        channel.AddToQueue(p);
                    }
                    length -= sendLength;
                }
                return;
            }
            
            var packet = _packetPool.GetPacket(headerSize + length);
            packet.Property = property;
            Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);

            if (channel == null)
            {
                lock (_unreliableChannel)
                {
                    _unreliableChannel.Enqueue(packet);
                }
            }
            else
            {
                channel.AddToQueue(packet);
            }
        }
        
        public void Disconnect(int code)
        {
            NetManager.DisconnectPeer(this, code, false);
        }

        internal DisconnectResult ProcessDisconnect(NetPacket packet)
        {
            if ((_connectionState == ConnectionState.Connected || _connectionState == ConnectionState.Outgoing) &&  packet.Size >= 9 && BitConverter.ToInt64(packet.RawData, 1) == _connectTime && packet.ConnectionNumber == _connectNum)
            {
                return _connectionState == ConnectionState.Connected  ? DisconnectResult.Disconnect : DisconnectResult.Reject;
            }
            return DisconnectResult.None;
        }

        internal ShutdownResult Shutdown(int code, bool force)
        {
            lock (_shutdownLock)
            {
                if (_connectionState == ConnectionState.Disconnected || _connectionState == ConnectionState.ShutdownRequested)
                {
                    return ShutdownResult.None;
                }

                var result = _connectionState == ConnectionState.Connected ? ShutdownResult.WasConnected : ShutdownResult.Success;

                if (force)
                {
                    _connectionState = ConnectionState.Disconnected;
                    return result;
                }
     
                Interlocked.Exchange(ref _timeSinceLastPacket, 0);

                _shutdownPacket = new NetPacket(PacketProperty.Disconnect, 4) {ConnectionNumber = _connectNum};
                FastBitConverter.GetBytes(_shutdownPacket.RawData, 1, _connectTime);
                Buffer.BlockCopy(BitConverter.GetBytes(code), 0, _shutdownPacket.RawData, 9, 4);
                _connectionState = ConnectionState.ShutdownRequested;
                NetManager.SendRaw(_shutdownPacket, EndPoint);
      
                return result;
            }
        }

        private void UpdateRoundTripTime(int roundTripTime)
        {
            _rtt += roundTripTime;
            _rttCount++;
            _avgRtt = _rtt/_rttCount;
            _resendDelay = 25.0 + _avgRtt * 2.1; // 25 ms + double rtt
        }

        internal void AddReliablePacket(DeliveryMethod method, NetPacket p)
        {
            if (p.IsFragmented)
            {
                ushort packetFragId = p.FragmentId;
                IncomingFragments incomingFragments;
                if (!_holdedFragments.TryGetValue(packetFragId, out incomingFragments))
                {
                    incomingFragments = new IncomingFragments
                    {
                        Fragments = new NetPacket[p.FragmentsTotal],
                        ChannelId = p.ChannelId
                    };
                    _holdedFragments.Add(packetFragId, incomingFragments);
                }

                var fragments = incomingFragments.Fragments;
                
                if (p.FragmentPart >= fragments.Length || fragments[p.FragmentPart] != null || p.ChannelId != incomingFragments.ChannelId)
                {
                    _packetPool.Recycle(p);
                    Bro.Log.Info("net peer :: invalid fragment packet");
                    return;
                }

                fragments[p.FragmentPart] = p;
                incomingFragments.ReceivedCount++;
                incomingFragments.TotalSize += p.Size - NetConstants.FragmentedHeaderTotalSize;
                if (incomingFragments.ReceivedCount != fragments.Length)
                {
                    return;
                }

                var resultingPacket = _packetPool.GetWithProperty( PacketProperty.Unreliable, incomingFragments.TotalSize);

                var firstFragmentSize = fragments[0].Size - NetConstants.FragmentedHeaderTotalSize;
                for (var i = 0; i < incomingFragments.ReceivedCount; i++)
                {
                    var fragment = fragments[i];
                    Buffer.BlockCopy( fragment.RawData, NetConstants.FragmentedHeaderTotalSize, resultingPacket.RawData, NetConstants.HeaderSize + firstFragmentSize * i, fragment.Size - NetConstants.FragmentedHeaderTotalSize);
                    _packetPool.Recycle(fragment);
                }
                
                Array.Clear(fragments, 0, incomingFragments.ReceivedCount);
                NetManager.CreateReceiveEvent(resultingPacket, this);
                _holdedFragments.Remove(packetFragId);
            }
            else 
            {
                NetManager.CreateReceiveEvent(p, this);
            }
        }

        private void ProcessMtuPacket(NetPacket packet)
        {
            if (packet.Size < NetConstants.PossibleMtu[0])
            {
                return;
            }

            var receivedMtu = BitConverter.ToInt32(packet.RawData, 1);
            var endMtuCheck = BitConverter.ToInt32(packet.RawData, packet.Size - 4);
            if (receivedMtu != packet.Size || receivedMtu != endMtuCheck || receivedMtu > NetConstants.MaxPacketSize)
            {
                Bro.Log.Info( string.Format( "net peer :: mtu broken packet. RMTU {0}, EMTU {1}, PSIZE {2}", receivedMtu, endMtuCheck, packet.Size ));
                return;
            }

            if (packet.Property == PacketProperty.MtuCheck)
            {
                _mtuCheckAttempts = 0;
                packet.Property = PacketProperty.MtuOk;
                NetManager.SendRawAndRecycle(packet, EndPoint);
            }
            else if(receivedMtu > _mtu && !_finishMtu)
            {
                if (receivedMtu != NetConstants.PossibleMtu[_mtuIdx + 1])
                {
                    return;
                }

                lock (_mtuMutex)
                {
                    _mtuIdx++;
                    SetMtu(_mtuIdx);
                }

                if (_mtuIdx == NetConstants.PossibleMtu.Length - 1)
                {
                    _finishMtu = true;
                }
            }
        }

        private void UpdateMtuLogic(int deltaTime)
        {
            if (_finishMtu)
            {
                return;
            }

            _mtuCheckTimer += deltaTime;
            if (_mtuCheckTimer < MtuCheckDelay)
            {
                return;
            }

            _mtuCheckTimer = 0;
            _mtuCheckAttempts++;
            if (_mtuCheckAttempts >= MaxMtuCheckAttempts)
            {
                _finishMtu = true;
                return;
            }

            lock (_mtuMutex)
            {
                if (_mtuIdx >= NetConstants.PossibleMtu.Length - 1)
                {
                    return;
                }

                var newMtu = NetConstants.PossibleMtu[_mtuIdx + 1];
                var p = _packetPool.GetPacket(newMtu);
                p.Property = PacketProperty.MtuCheck;
                FastBitConverter.GetBytes(p.RawData, 1, newMtu);         //place into start
                FastBitConverter.GetBytes(p.RawData, p.Size - 4, newMtu);//and end of packet

                if (NetManager.SendRawAndRecycle(p, EndPoint) <= 0)
                {
                    _finishMtu = true;
                }
            }
        }

        internal ConnectRequestResult ProcessConnectRequest(NetConnectRequestPacket connRequest)
        {
            switch (_connectionState)
            {
                case ConnectionState.Outgoing:
                    if (connRequest.ConnectionTime >= _connectTime)
                    {
                        _connectTime = connRequest.ConnectionTime;
                        ConnectionNum = connRequest.ConnectionNumber;
                    }
                    return ConnectRequestResult.P2PConnection;

                case ConnectionState.Connected:
                    if (connRequest.ConnectionTime == _connectTime)
                    {
                        NetManager.SendRaw(_connectAcceptPacket, EndPoint);
                    }

                    else if (connRequest.ConnectionTime > _connectTime)
                    {
                        return ConnectRequestResult.Reconnection;
                    }
                    break;

                case ConnectionState.Disconnected:
                case ConnectionState.ShutdownRequested:
                    if (connRequest.ConnectionTime >= _connectTime)
                    {
                        return ConnectRequestResult.NewConnection;
                    }
                    break;
            }
            return ConnectRequestResult.None;
        }
        
        internal void ProcessPacket(NetPacket packet)
        {
            if (_connectionState == ConnectionState.Outgoing)
            {
                _packetPool.Recycle(packet);
                return;
            }
            if (packet.ConnectionNumber != _connectNum && packet.Property != PacketProperty.ShutdownOk) //without connectionNum
            {
                _packetPool.Recycle(packet);
                return;
            }
            
            Interlocked.Exchange(ref _timeSinceLastPacket, 0);
            
            switch (packet.Property)
            {
                case PacketProperty.Merged:
                    var pos = NetConstants.HeaderSize;
                    while (pos < packet.Size)
                    {
                        var size = BitConverter.ToUInt16(packet.RawData, pos);
                        pos += 2;
                        var mergedPacket = _packetPool.GetPacket(size);
                        if (!mergedPacket.FromBytes(packet.RawData, pos, size))
                        {
                            _packetPool.Recycle(packet);
                            break;
                        }
                        pos += size;
                        ProcessPacket(mergedPacket);
                    }
                    break;
                case PacketProperty.Ping:
                    if (NetUtils.RelativeSequenceNumber(packet.Sequence, _pongPacket.Sequence) > 0)
                    {
                        FastBitConverter.GetBytes(_pongPacket.RawData, 3, DateTime.UtcNow.Ticks);
                        _pongPacket.Sequence = packet.Sequence;
                        NetManager.SendRaw(_pongPacket, EndPoint);
                    }
                    _packetPool.Recycle(packet);
                    break;
                
                case PacketProperty.Pong:
                    if (packet.Sequence == _pingPacket.Sequence)
                    {
                        _pingTimer.Stop();
                        var elapsedMs = (int)_pingTimer.ElapsedMilliseconds;
                        UpdateRoundTripTime(elapsedMs);
                    }
                    _packetPool.Recycle(packet);
                    break;

                case PacketProperty.Ack:
                case PacketProperty.Channeled:
                    if (packet.ChannelId > _channels.Length)
                    {
                        _packetPool.Recycle(packet);
                        break;
                    }
                    var channel = _channels[packet.ChannelId] ?? (packet.Property == PacketProperty.Ack ? null : CreateChannel(packet.ChannelId));
                    if (channel != null)
                    {
                        if (!channel.ProcessPacket(packet))
                            _packetPool.Recycle(packet);
                    }
                    break;
                
                case PacketProperty.Unreliable:
                    NetManager.CreateReceiveEvent(packet, this);
                    return;

                case PacketProperty.MtuCheck:
                case PacketProperty.MtuOk:
                    ProcessMtuPacket(packet);
                    break;

                case PacketProperty.ShutdownOk:
                    if(_connectionState == ConnectionState.ShutdownRequested)
                        _connectionState = ConnectionState.Disconnected;
                    _packetPool.Recycle(packet);
                    break;            
                
                default:
                    Bro.Log.Info("net peer :: unexpected packet type: " + packet.Property);
                    break;
            }
        }

        private void SendMerged()
        {
            if (_mergeCount == 0)
            {
                return;
            }

            if (_mergeCount > 1)
            {
                NetManager.SendRaw(_mergeData.RawData, 0, NetConstants.HeaderSize + _mergePos, EndPoint);
            }
            else
            { 
                NetManager.SendRaw(_mergeData.RawData, NetConstants.HeaderSize + 2, _mergePos - 2, EndPoint);
            }

            _mergePos = 0;
            _mergeCount = 0;
        }

        internal void SendUserData(NetPacket packet)
        {
            packet.ConnectionNumber = _connectNum;
            var mergedPacketSize = NetConstants.HeaderSize + packet.Size + 2;
            const int sizeThreshold = 20;
            if (mergedPacketSize + sizeThreshold >= _mtu)
            {
                NetManager.SendRaw(packet, EndPoint);
                return;
            }

            if (_mergePos + mergedPacketSize > _mtu)
            {
                SendMerged();
            }

            FastBitConverter.GetBytes(_mergeData.RawData, _mergePos + NetConstants.HeaderSize, (ushort)packet.Size);
            Buffer.BlockCopy(packet.RawData, 0, _mergeData.RawData, _mergePos + NetConstants.HeaderSize + 2, packet.Size);
            _mergePos += packet.Size + 2;
            _mergeCount++;
        }
        
        public void Flush()
        {
            if (_connectionState != ConnectionState.Connected)
            {
                return;
            }

            lock (_flushLock)
            {
                var currentChannel = _headChannel;
                while (currentChannel != null)
                {
                    currentChannel.SendNextPackets();
                    currentChannel = currentChannel.Next;
                }

                lock (_unreliableChannel)
                {
                    while (_unreliableChannel.Count > 0)
                    {
                        var packet = _unreliableChannel.Dequeue();
                        SendUserData(packet);
                        NetManager.NetPacketPool.Recycle(packet);
                    }
                }

                SendMerged();
            }
        }

        internal void Update(int deltaTime)
        {
            Interlocked.Add(ref _timeSinceLastPacket, deltaTime);
            switch (_connectionState)
            {
                case ConnectionState.Connected:
                    if (_timeSinceLastPacket > NetManager.DisconnectTimeout)
                    {
                        Bro.Log.Info($"net peer :: disconnect time since last packet = {_timeSinceLastPacket}");
                        NetManager.DisconnectPeer(this, DisconnectCode.Timeout, true);
                        return;
                    }
                    break;

                case ConnectionState.ShutdownRequested:
                    if (_timeSinceLastPacket > NetManager.DisconnectTimeout)
                    {
                        _connectionState = ConnectionState.Disconnected;
                    }
                    else
                    {
                        _shutdownTimer += deltaTime;
                        if (_shutdownTimer >= ShutdownDelay)
                        {
                            _shutdownTimer = 0;
                            NetManager.SendRaw(_shutdownPacket, EndPoint);
                        }
                    }
                    return;

                case ConnectionState.Outgoing:
                    _connectTimer += deltaTime;
                    if (_connectTimer > NetManager.ReconnectDelay)
                    {
                        _connectTimer = 0;
                        _connectAttempts++;
                        if (_connectAttempts > NetManager.MaxConnectAttempts)
                        {
                            NetManager.DisconnectPeer(this, DisconnectCode.ConnectionFailed, true);
                            return;
                        }
                        
                        NetManager.SendRaw(_connectRequestPacket, EndPoint);
                    }
                    return;

                case ConnectionState.Disconnected:
                    return;
            }
            
            _pingSendTimer += deltaTime;
            if (_pingSendTimer >= NetManager.PingInterval)
            {
                _pingSendTimer = 0;
                _pingPacket.Sequence++;
                if (_pingTimer.IsRunning)
                {
                    UpdateRoundTripTime((int) _pingTimer.ElapsedMilliseconds);
                }
                _pingTimer.Reset();
                _pingTimer.Start();
                NetManager.SendRaw(_pingPacket, EndPoint);
            }
            
            _rttResetTimer += deltaTime;
            if (_rttResetTimer >= NetManager.PingInterval * 3)
            {
                _rttResetTimer = 0;
                _rtt = _avgRtt;
                _rttCount = 1;
            }

            UpdateMtuLogic(deltaTime);
            Flush();
        }

        internal void RecycleAndDeliver(NetPacket packet)
        {
            _packetPool.Recycle(packet);
        }
    }
}