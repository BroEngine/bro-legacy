using System;

namespace Bro.Network.TransmitProtocol
{
    public class BinaryCacheReader : IReader
    {
        private readonly System.IO.MemoryStream _stream;
        private readonly System.IO.BinaryReader _reader;
        private readonly byte[] _data;

        public BinaryCacheReader(byte[] data)
        {
            _data = data;
            _stream = new System.IO.MemoryStream(data);
            _reader = new System.IO.BinaryReader(_stream);
        }
           
        public BinaryCacheReader()
        {
            _stream = new System.IO.MemoryStream();
            _reader = new System.IO.BinaryReader(_stream);
        }
        
        public BinaryCacheReader(byte[] data, int size)
        {
            _data = data;
            _stream = new System.IO.MemoryStream(data, 0, size);
            _reader = new System.IO.BinaryReader(_stream);
        }

        public void Write(byte[] data, int size)
        {
            _reader.BaseStream.SetLength(0);
            _reader.BaseStream.Write(data, 0, size);
            _reader.BaseStream.Position = 0;
        }

        public void Dispose()
        {
            //_reader.Dispose();
            _stream.Dispose();
        }

        public void Read(out bool value)
        {
            value = _reader.ReadBoolean();
        }

        public void Read(out int value)
        {
            value = _reader.ReadInt32();
        }

        public void Read(out long value)
        {
            value = _reader.ReadInt64();
        }

        public void Read(out double value)
        {
            value = _reader.ReadDouble();
        }

        public void Read(out float value)
        {
            value = _reader.ReadSingle();
        }

        public void Read(out short value)
        {
            value = _reader.ReadInt16();
        }

        public void Read(out byte value)
        {
            value = _reader.ReadByte();
        }

        public void Read(out string value)
        {
            value = _reader.ReadString();
        }

        public void Read(out byte[] value, int size)
        {
            value = _reader.ReadBytes(size);
        } 
        
        public void Read(byte[] value, int size)
        {
            _reader.Read(value, 0, size);
        }

        public void Reset()
        {
            _reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        public bool IsEndOfData => (int) _reader.BaseStream.Position >= _reader.BaseStream.Length;

        public long Position
        {
            get => _reader.BaseStream.Position;
            set => _reader.BaseStream.Position = value;
        }

        public override string ToString()
        {
            return $"{base.ToString()} base64 = {Convert.ToBase64String(_data)}";
        }
    }
}