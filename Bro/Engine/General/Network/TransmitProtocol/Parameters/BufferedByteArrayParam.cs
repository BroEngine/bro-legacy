using System;

namespace Bro.Network.TransmitProtocol
{
    public class BufferedByteArrayParam : BaseParam
    {
        private readonly int _maxSize;
        private readonly byte[] _array;
        private int _size;

        public int Size => _size;

        public void Set(byte[] data, int size)
        {
            IsInitialized = true;
            Buffer.BlockCopy(data, 0, _array, 0, size);
            _size = size;
        }

        public int Get(byte[] data)
        {
            CheckInitialized();
            Buffer.BlockCopy(_array, 0, data, 0, _size);
            return _size;
        }

        public BufferedByteArrayParam(int maxSize, bool isOptional = false) : base(isOptional)
        {
            _maxSize = maxSize;
            _array = new byte[maxSize];
        }

        public override void Write(IWriter writer)
        {
            if (_size > _maxSize)
            {
                throw new System.ArgumentException("Wrong array size");
            }

            if (_maxSize <= byte.MaxValue)
            {
                writer.Write((byte) _size);
            }
            else if (_maxSize <= short.MaxValue)
            {
                writer.Write((short) _size);
            }
            else
            {
                writer.Write((int) _size);
            }
            writer.Write(_array, _size);
        }

        public override void Read(IReader reader)
        {
            if (_maxSize <= byte.MaxValue)
            {
                reader.Read(out byte sizeValue);
                _size = sizeValue;
            }
            else if (_maxSize <= short.MaxValue)
            {
                reader.Read(out short sizeValue);
                _size = sizeValue;
            }
            else
            {
                reader.Read(out int sizeValue);
                _size = sizeValue;
            }

            reader.Read(out var a, _size);
            Buffer.BlockCopy(a, 0, _array, 0, _size);
            
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}