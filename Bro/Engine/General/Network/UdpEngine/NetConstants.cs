// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------

namespace Bro.Network.Udp.Engine
{
    public enum DeliveryMethod : byte
    {
        Unreliable = 4,
        ReliableUnordered = 0,
        Sequenced = 1,
        ReliableOrdered = 2,
        ReliableSequenced = 3
    }
    
    public static class NetConstants
    {
        public const int DefaultWindowSize = 64;
        public const int SocketBufferSize = 1024 * 1024; //1mb
        public const int SocketTtl = 255;
        
        public const int HeaderSize = 1;
        public const int ChanneledHeaderSize = 4;
        public const int FragmentHeaderSize = 6;
        public const int FragmentedHeaderTotalSize = ChanneledHeaderSize + FragmentHeaderSize;
        public const ushort MaxSequence = 32768;
        public const ushort HalfMaxSequence = MaxSequence / 2;
        
        internal const int ProtocolId = 10;
        internal const int MaxUdpHeaderSize = 68;

        internal static readonly int[] PossibleMtu =
        {
            576  - MaxUdpHeaderSize, //minimal
            1232 - MaxUdpHeaderSize,
            1460 - MaxUdpHeaderSize, //google cloud
            1472 - MaxUdpHeaderSize, //VPN
            1492 - MaxUdpHeaderSize, //Ethernet with LLC and SNAP, PPPoE (RFC 1042)
            1500 - MaxUdpHeaderSize  //Ethernet II (RFC 1191)
        };

        internal static readonly int MaxPacketSize = PossibleMtu[PossibleMtu.Length - 1];
        public const byte MaxConnectionNumber = 4;
        public const int PacketPoolSize = 1000;
    }
}
