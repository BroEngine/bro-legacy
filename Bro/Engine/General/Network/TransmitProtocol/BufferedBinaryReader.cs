using System;
using System.IO;
using System.Text;

namespace Bro.Network.TransmitProtocol
{
    public class BufferedBinaryReader : IDisposable
    {
        private readonly Stream stream;
        private readonly byte[] buffer;
        private readonly int bufferSize;
        private int bufferOffset;
        private int numBufferedBytes;

        public BufferedBinaryReader(Stream stream, int bufferSize = 4096)
        {
            this.stream = stream;
            this.bufferSize = bufferSize;
            buffer = new byte[bufferSize];
            bufferOffset = bufferSize;
        }

        public int NumBytesAvailable
        {
            get { return System.Math.Max(0, numBufferedBytes - bufferOffset); }
        }

        public bool FillBuffer()
        {
            var numBytesUnread = bufferSize - bufferOffset;
            var numBytesToRead = bufferSize - numBytesUnread;
            bufferOffset = 0;
            numBufferedBytes = numBytesUnread;
            if (numBytesUnread > 0)
            {
                Buffer.BlockCopy(buffer, numBytesToRead, buffer, 0, numBytesUnread);
            }

            while (numBytesToRead > 0)
            {
                var numBytesRead = stream.Read(buffer, numBytesUnread, numBytesToRead);
                if (numBytesRead == 0)
                {
                    return false;
                }

                numBufferedBytes += numBytesRead;
                numBytesToRead -= numBytesRead;
                numBytesUnread += numBytesRead;
            }

            return true;
        }

        public byte[] ReadArray(int size)
        {
            byte[] array = new byte[size];
            for (int i = 0; i < size; ++i)
            {
                array[i] = ReadByte();
            }

            return array;
        }

        public byte ReadByte()
        {
            var val = buffer[bufferOffset];
            bufferOffset += 1;
            return val;
        }

        public short ReadInt16()
        {
            var val = (short) (buffer[bufferOffset] | buffer[bufferOffset + 1] << 8);
            bufferOffset += 2;
            return val;
        }

        public int ReadInt32()
        {
            var val = (int) (buffer[bufferOffset] | buffer[bufferOffset + 1] << 8 | buffer[bufferOffset + 2] << 16 |
                             buffer[bufferOffset + 3] << 24);
            bufferOffset += 4;
            return val;
        }

        [System.Security.SecuritySafeCritical] // auto-generated
        public virtual unsafe float ReadSingle()
        {
            uint tmpBuffer = (uint) (buffer[bufferOffset] | buffer[bufferOffset + 1] << 8 |
                                     buffer[bufferOffset + 2] << 16 | buffer[bufferOffset + 3] << 24);
            bufferOffset += 4;
            return *((float*) &tmpBuffer);
        }

        public long ReadInt64()
        {
            uint lo = (uint) (buffer[bufferOffset] | buffer[bufferOffset + 1] << 8 |
                              buffer[bufferOffset + 2] << 16 | buffer[bufferOffset + 3] << 24);
            uint hi = (uint) (buffer[bufferOffset + 4] | buffer[bufferOffset + 5] << 8 |
                              buffer[bufferOffset + 6] << 16 | buffer[bufferOffset + 7] << 24);
            bufferOffset += 8;
            return (long) ((ulong) hi) << 32 | lo;
        }


        internal protected int Read7BitEncodedInt()
        {
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                b = ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return count;
        }

        public void Dispose()
        {
            stream.Close();
        }

        public virtual String ReadString()
        {
            int stringLength = Read7BitEncodedInt();

            byte[] data = new byte[stringLength];

            for (int i = 0; i < stringLength; ++i)
            {
                data[i] = buffer[bufferOffset + i];
            }

            bufferOffset += stringLength;
            return System.Text.Encoding.Default.GetString(data);
        }


        public string ReadUTF8()
        {
            int l = ReadInt16();
            return Encoding.UTF8.GetString(ReadArray(l));
        }
    }
}