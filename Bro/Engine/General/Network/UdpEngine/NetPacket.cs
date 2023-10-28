// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------

using System;

namespace Bro.Network.Udp.Engine
{
    internal enum PacketProperty : byte
    {
        Unreliable,
        Channeled,
        Ack,
        Ping,
        Pong,
        ConnectRequest,
        ConnectAccept,
        Disconnect,
        MtuCheck,
        MtuOk,
        Merged,
        ShutdownOk,
        PeerNotFound,
        InvalidProtocol
    }

    internal sealed class NetPacket
    {
        private static readonly int LastProperty = Enum.GetValues(typeof(PacketProperty)).Length;

        public PacketProperty Property
        {
            get { return (PacketProperty)(RawData[0] & 0x1F); }
            set { RawData[0] = (byte)((RawData[0] & 0xE0) | (byte)value); }
        }

        public byte ConnectionNumber
        {
            get { return (byte)((RawData[0] & 0x60) >> 5); }
            set { RawData[0] = (byte) ((RawData[0] & 0x9F) | (value << 5)); }
        }

        public ushort Sequence
        {
            get { return BitConverter.ToUInt16(RawData, 1); }
            set { FastBitConverter.GetBytes(RawData, 1, value); }
        }

        public bool IsFragmented
        {
            get { return (RawData[0] & 0x80) != 0; }
        }

        public void MarkFragmented()
        {
            RawData[0] |= 0x80; //set first bit
        }

        public byte ChannelId
        {
            get { return RawData[3]; }
            set { RawData[3] = value; }
        }

        public ushort FragmentId
        {
            get { return BitConverter.ToUInt16(RawData, 4); }
            set { FastBitConverter.GetBytes(RawData, 4, value); }
        }

        public ushort FragmentPart
        {
            get { return BitConverter.ToUInt16(RawData, 6); }
            set { FastBitConverter.GetBytes(RawData, 6, value); }
        }

        public ushort FragmentsTotal
        {
            get { return BitConverter.ToUInt16(RawData, 8); }
            set { FastBitConverter.GetBytes(RawData, 8, value); }
        }

        public byte[] RawData;
        public int Size;
        
        public NetPacket(int size)
        {
            RawData = new byte[size];
            Size = size;
        }

        public NetPacket(PacketProperty property, int size)
        {
            size += GetHeaderSize(property);
            RawData = new byte[size];
            Property = property;
            Size = size;
        }

        public byte[] GetPacketData()
        {
            var headerSize = GetHeaderSize();
            var dataSize = Size - headerSize;
            var data = new byte[dataSize];
            Buffer.BlockCopy(RawData, headerSize, data, 0, dataSize);
            return data;
        }
        
        public int GetIntegerCode()
        {
            try
            {
                var data = GetPacketData();
                if (data != null && data.Length == 4)
                {
                    return BitConverter.ToInt32( data , 0 );
                }
            }
            catch { /* */ }
            return 0;
        }
        
        public static int GetHeaderSize(PacketProperty property)
        {
            switch (property)
            {
                case PacketProperty.Channeled:
                case PacketProperty.Ack:
                    return NetConstants.ChanneledHeaderSize;
                case PacketProperty.Ping:
                    return NetConstants.HeaderSize + 2;
                case PacketProperty.ConnectRequest:
                    return NetConnectRequestPacket.HeaderSize;
                case PacketProperty.ConnectAccept:
                    return NetConnectAcceptPacket.Size;
                case PacketProperty.Disconnect:
                    return NetConstants.HeaderSize + 8;
                case PacketProperty.Pong:
                    return NetConstants.HeaderSize + 10;
                default:
                    return NetConstants.HeaderSize;
            }
        }

        public int GetHeaderSize()
        {
            return GetHeaderSize(Property);
        }

        public bool FromBytes(byte[] data, int start, int packetSize)
        {
            var property = (byte)(data[start] & 0x1F);
            var fragmented = (data[start] & 0x80) != 0;
            var headerSize = GetHeaderSize((PacketProperty) property);

            if (property > LastProperty || packetSize < headerSize || (fragmented && packetSize < headerSize + NetConstants.FragmentHeaderSize) || data.Length < start + packetSize)
            {
                return false;
            }

            Buffer.BlockCopy(data, start, RawData, 0, packetSize);
            Size = (ushort)packetSize;
            return true;
        }
    }

    internal sealed class NetConnectRequestPacket
    {
        public const int HeaderSize = 13;
        public readonly long ConnectionTime;
        public readonly byte ConnectionNumber;
        
        private NetConnectRequestPacket(long connectionId, byte connectionNumber)
        {
            ConnectionTime = connectionId;
            ConnectionNumber = connectionNumber;
        }

        public static int GetProtocolId(NetPacket packet)
        {
            return BitConverter.ToInt32(packet.RawData, 1);
        }
        
        public static NetConnectRequestPacket FromData(NetPacket packet)
        {
            if (packet.ConnectionNumber >= NetConstants.MaxConnectionNumber)
            {
                return null;
            }

            var connectionId = BitConverter.ToInt64(packet.RawData, 5);
            return new NetConnectRequestPacket(connectionId, packet.ConnectionNumber);
        }

        public static NetPacket Make(long connectId)
        {
            var packet = new NetPacket(PacketProperty.ConnectRequest, 0);
            FastBitConverter.GetBytes(packet.RawData, 1, NetConstants.ProtocolId);
            FastBitConverter.GetBytes(packet.RawData, 5, connectId);
            
            return packet;
        }
    }

    internal sealed class NetConnectAcceptPacket
    {
        public const int Size = 11;
        public readonly long ConnectionId;
        public readonly byte ConnectionNumber;

        private NetConnectAcceptPacket(long connectionId, byte connectionNumber)
        {
            ConnectionId = connectionId;
            ConnectionNumber = connectionNumber;
        }

        public static NetConnectAcceptPacket FromData(NetPacket packet)
        {
            if (packet.Size > Size)
            {
                return null;
            }

            var connectionId = BitConverter.ToInt64(packet.RawData, 1);
            var connectionNumber = packet.RawData[9];
            if (connectionNumber >= NetConstants.MaxConnectionNumber)
            {
                return null;
            }

            var isReused = packet.RawData[10];
            if (isReused > 1)
            {
                return null;
            }

            return new NetConnectAcceptPacket(connectionId, connectionNumber);
        }

        public static NetPacket Make(long connectId, byte connectNum, bool reusedPeer)
        {
            var packet = new NetPacket(PacketProperty.ConnectAccept, 0);
            FastBitConverter.GetBytes(packet.RawData, 1, connectId);
            packet.RawData[9] = connectNum;
            packet.RawData[10] = (byte)(reusedPeer ? 1 : 0);
            return packet;
        }
    }
}