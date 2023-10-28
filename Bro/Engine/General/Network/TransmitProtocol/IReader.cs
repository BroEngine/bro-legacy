using System;

namespace Bro.Network.TransmitProtocol
{
    public interface IReader : IDisposable
    {
        void Read(out bool value);

        void Read(out long value);

        void Read(out double value);

        void Read(out float value);

        void Read(out int value);

        void Read(out short value);

        void Read(out byte value);

        void Read(out string value);

        void Read(out byte[] value, int size);
        
        void Read(byte[] buffer, int size);  

        long Position { get; set; }

        bool IsEndOfData { get; }

        void Reset();
    }
}