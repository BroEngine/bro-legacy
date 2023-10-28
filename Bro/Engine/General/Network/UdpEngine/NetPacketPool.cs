// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------


using System.Threading;

namespace Bro.Network.Udp.Engine
{
    internal sealed class NetPacketPool
    {
        private readonly NetPacket[] _pool = new NetPacket[NetConstants.PacketPoolSize];
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private int _count;

        public NetPacket GetWithProperty(PacketProperty property, int size)
        {
            var packet = GetPacket(size + NetPacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        public NetPacket GetWithProperty(PacketProperty property)
        {
            var packet = GetPacket(NetPacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        public NetPacket GetPacket(int size)
        {
            if (size <= NetConstants.MaxPacketSize)
            {
                NetPacket packet = null;
                _lock.EnterUpgradeableReadLock();
                if (_count > 0)
                {
                    _lock.EnterWriteLock();
                    _count--;
                    packet = _pool[_count];
                    _pool[_count] = null;
                    _lock.ExitWriteLock();
                }
                _lock.ExitUpgradeableReadLock();
                if (packet != null)
                {
                    packet.Size = size;
                    if (packet.RawData.Length < size)
                    {
                        packet.RawData = new byte[size];
                    }
                    return packet;
                }
            }
            return new NetPacket(size);
        }

        public void Recycle(NetPacket packet)
        {
            if (packet == null || packet.RawData.Length > NetConstants.MaxPacketSize)
            {
                return;
            }
            
            packet.RawData[0] = 0;

            _lock.EnterUpgradeableReadLock();
            if (_count == NetConstants.PacketPoolSize)
            {
                _lock.ExitUpgradeableReadLock();
                return;
            }
            _lock.EnterWriteLock();
            _pool[_count] = packet;
            _count++;
            _lock.ExitWriteLock();
            _lock.ExitUpgradeableReadLock();
        }
    }
}