using System;

namespace Bro.Network.Udp.Engine
{
    internal sealed class ReliableChannel : BaseChannel
    {
        private struct PendingPacket
        {
            private NetPacket _packet;
            private long _timeStamp;
            private bool _isSent;

            public override string ToString()
            {
                return _packet == null ? "Empty" : _packet.Sequence.ToString();
            }

            public void Init(NetPacket packet)
            {
                _packet = packet;
                _isSent = false;
            }

            public void TrySend(long currentTime, NetPeer peer)
            {
                if (_packet == null)
                {
                    return;
                }

                if (_isSent)
                {
                    double resendDelay = peer.ResendDelay * TimeSpan.TicksPerMillisecond;
                    double packetHoldTime = currentTime - _timeStamp;
                    if (packetHoldTime < resendDelay)
                    {
                        return;
                    }
                }
                _timeStamp = currentTime;
                _isSent = true;
                peer.SendUserData(_packet);
            }

            public bool Clear(NetPeer peer)
            {
                if (_packet != null)
                {
                    peer.RecycleAndDeliver(_packet);
                    _packet = null;
                    return true;
                }
                return false;
            }
        }

        private readonly NetPacket _outgoingAcks;            //for send acks
        private readonly PendingPacket[] _pendingPackets;    //for unacked packets and duplicates
        private readonly NetPacket[] _receivedPackets;       //for order
        private readonly bool[] _earlyReceived;              //for unordered

        private int _localSequence;
        private int _remoteSequence;
        private int _localWindowStart;
        private int _remoteWindowStart;

        private bool _mustSendAcks;

        private readonly DeliveryMethod _deliveryMethod;
        private readonly bool _ordered;
        private readonly int _windowSize;
        private const int BitsInByte = 8;
        private readonly byte _id;

        public ReliableChannel(NetPeer peer, bool ordered, byte id) : base(peer)
        {
            _id = id;
            _windowSize = NetConstants.DefaultWindowSize;
            _ordered = ordered;
            _pendingPackets = new PendingPacket[_windowSize];
            
            for (int i = 0; i < _pendingPackets.Length; i++)
            {
                _pendingPackets[i] = new PendingPacket();
            }

            if (_ordered)
            {
                _deliveryMethod = DeliveryMethod.ReliableOrdered;
                _receivedPackets = new NetPacket[_windowSize];
            }
            else
            {
                _deliveryMethod = DeliveryMethod.ReliableUnordered;
                _earlyReceived = new bool[_windowSize];
            }

            _localWindowStart = 0;
            _localSequence = 0;
            _remoteSequence = 0;
            _remoteWindowStart = 0;
            _outgoingAcks = new NetPacket(PacketProperty.Ack, (_windowSize - 1) / BitsInByte + 2) {ChannelId = id};
        }
        
        private void ProcessAck(NetPacket packet)
        {
            if (packet.Size != _outgoingAcks.Size)
            {
                return;
            }

            var ackWindowStart = packet.Sequence;
            var windowRel = NetUtils.RelativeSequenceNumber(_localWindowStart, ackWindowStart);
            if (ackWindowStart >= NetConstants.MaxSequence || windowRel < 0)
            {
                return;
            }
  
            if (windowRel >= _windowSize)
            {
                return;
            }

            var acksData = packet.RawData;
            lock (_pendingPackets)
            {
                for (var pendingSeq = _localWindowStart; pendingSeq != _localSequence; pendingSeq = (pendingSeq + 1) % NetConstants.MaxSequence)
                {
                    var rel = NetUtils.RelativeSequenceNumber(pendingSeq, ackWindowStart);
                    if (rel >= _windowSize)
                    {
                        break;
                    }

                    var pendingIdx = pendingSeq % _windowSize;
                    var currentByte = NetConstants.ChanneledHeaderSize + pendingIdx / BitsInByte;
                    var currentBit = pendingIdx % BitsInByte;
                    if ((acksData[currentByte] & (1 << currentBit)) == 0)
                    {
                        continue;
                    }

                    if (pendingSeq == _localWindowStart)
                    {
                        _localWindowStart = (_localWindowStart + 1) % NetConstants.MaxSequence;
                    }

                    _pendingPackets[pendingIdx].Clear(Peer);
                }
            }
        }

        public override void SendNextPackets()
        {
            if (_mustSendAcks)
            {
                _mustSendAcks = false;
                lock (_outgoingAcks)
                {
                    Peer.SendUserData(_outgoingAcks);
                }
            }

            var currentTime = DateTime.UtcNow.Ticks;
            lock (_pendingPackets)
            {
                lock (OutgoingQueue)
                {
                    while (OutgoingQueue.Count > 0)
                    {
                        var relate = NetUtils.RelativeSequenceNumber(_localSequence, _localWindowStart);
                        if (relate >= _windowSize)
                        {
                            break;
                        }

                        var netPacket = OutgoingQueue.Dequeue();
                        netPacket.Sequence = (ushort) _localSequence;
                        netPacket.ChannelId = _id;
                        _pendingPackets[_localSequence % _windowSize].Init(netPacket);
                        _localSequence = (_localSequence + 1) % NetConstants.MaxSequence;
                    }
                }

                for (var pendingSeq = _localWindowStart; pendingSeq != _localSequence; pendingSeq = (pendingSeq + 1) % NetConstants.MaxSequence)
                {
                    _pendingPackets[pendingSeq % _windowSize].TrySend(currentTime, Peer);
                }
            }
        }

        public override bool ProcessPacket(NetPacket packet)
        {
            if (packet.Property == PacketProperty.Ack)
            {
                ProcessAck(packet);
                return false;
            }
            
            int seq = packet.Sequence;
            if (seq >= NetConstants.MaxSequence)
            {
                return false;
            }

            var relate = NetUtils.RelativeSequenceNumber(seq, _remoteWindowStart);
            var relateSeq = NetUtils.RelativeSequenceNumber(seq, _remoteSequence);

            if (relateSeq > _windowSize)
            {
                return false;
            }
            
            if (relate < 0)
            {
                return false;
            }
            if (relate >= _windowSize * 2)
            {
                return false;
            }

            int ackIdx;
            int ackByte;
            int ackBit;
            lock (_outgoingAcks)
            {
                if (relate >= _windowSize)
                {
                    var newWindowStart = (_remoteWindowStart + relate - _windowSize + 1) % NetConstants.MaxSequence;
                    _outgoingAcks.Sequence = (ushort) newWindowStart;
                    
                    while (_remoteWindowStart != newWindowStart)
                    {
                        ackIdx = _remoteWindowStart % _windowSize;
                        ackByte = NetConstants.ChanneledHeaderSize + ackIdx / BitsInByte;
                        ackBit = ackIdx % BitsInByte;
                        _outgoingAcks.RawData[ackByte] &= (byte) ~(1 << ackBit);
                        _remoteWindowStart = (_remoteWindowStart + 1) % NetConstants.MaxSequence;
                    }
                }

                _mustSendAcks = true;
                ackIdx = seq % _windowSize;
                ackByte = NetConstants.ChanneledHeaderSize + ackIdx / BitsInByte;
                ackBit = ackIdx % BitsInByte;
                if ((_outgoingAcks.RawData[ackByte] & (1 << ackBit)) != 0)
                {
                    return false;
                }

                _outgoingAcks.RawData[ackByte] |= (byte) (1 << ackBit);
            }
            
            if (seq == _remoteSequence)
            {
                Peer.AddReliablePacket(_deliveryMethod, packet);
                _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;

                if (_ordered)
                {
                    NetPacket p;
                    while ((p = _receivedPackets[_remoteSequence % _windowSize]) != null)
                    {
                        _receivedPackets[_remoteSequence % _windowSize] = null;
                        Peer.AddReliablePacket(_deliveryMethod, p);
                        _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;
                    }
                }
                else
                {
                    while (_earlyReceived[_remoteSequence % _windowSize])
                    {
                        _earlyReceived[_remoteSequence % _windowSize] = false;
                        _remoteSequence = (_remoteSequence + 1) % NetConstants.MaxSequence;
                    }
                }
                return true;
            }

            if (_ordered)
            {
                _receivedPackets[ackIdx] = packet;
            }
            else
            {
                _earlyReceived[ackIdx] = true;
                Peer.AddReliablePacket(_deliveryMethod, packet);
            }
            return true;
        }
    }
}