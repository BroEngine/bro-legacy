using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bro.Network.TransmitProtocol
{
    public class BinaryCacheWriter : IWriter
    {
        private byte[] _buffer;
        private readonly System.IO.MemoryStream _stream;
        private readonly System.IO.BinaryWriter _streamWriter;
        

        public BinaryCacheWriter(int maxSizeInBytes)
        {
            _buffer = new byte[maxSizeInBytes];
            _stream = new System.IO.MemoryStream(_buffer);
            _streamWriter = new System.IO.BinaryWriter(_stream);
        }
        
        public BinaryCacheWriter(byte[] buffer)
        {
            _buffer = buffer;
            _stream = new System.IO.MemoryStream(_buffer);
            _streamWriter = new System.IO.BinaryWriter(_stream);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _buffer = null;
        }

        public byte[] Data
        {
            get
            {
                if (_stream.CanRead)
                {
                    var dataSize = (int) _stream.Position;
                    var result = new byte[dataSize];
                    if (_buffer != null)
                    {
                        Buffer.BlockCopy(_buffer, 0, result, 0, dataSize);
                    }

                    return result;
                }

                return null;
            }
        }

        public long Position
        {
            get => _stream.CanRead ? _streamWriter.BaseStream.Position : 0;
            set
            {
                if (_stream.CanRead)
                {
                    _streamWriter.BaseStream.Position = value;
                }
            }
        }

        public void Reset()
        {
            if (_stream.CanRead)
            {
                _streamWriter.Seek(0, System.IO.SeekOrigin.Begin);
            }
        }

        public void Write(bool value)
        {
            _streamWriter.Write(value);
        }

        public void Write(float value)
        {
            _streamWriter.Write(value);
        }

        public void Write(double value)
        {
            _streamWriter.Write(value);
        }

        public void Write(int value)
        {
            _streamWriter.Write(value);
        }

        public void Write(long value)
        {
            _streamWriter.Write(value);
        }

        public void Write(short value)
        {
            _streamWriter.Write(value);
        }

        public void Write(byte value)
        {
            _streamWriter.Write(value);
        }

        public void Write(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            _streamWriter.Write(value);
        }

        public void Write(byte[] value)
        {
            _streamWriter.Write(value);
        }
        
        public void Write(byte[] buffer, int size)
        {
            _streamWriter.Write(buffer,0, size);   
        }
        
        public void WriteUTF8(string str)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(str);
            Write((short) nameBytes.Length);
            Write(nameBytes);
        }

        public int Size => Data.Length;
    }
}