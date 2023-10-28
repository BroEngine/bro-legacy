namespace Bro.Network.TransmitProtocol
{
    public interface IWriter : System.IDisposable
    {
        void Write(bool value);

        void Write(float value);

        void Write(double value);

        void Write(long value);

        void Write(int value);

        void Write(short value);

        void Write(byte value);

        void Write(string value);

        void Write(byte[] value);
        
        void Write(byte[] buffer, int size);

        void Reset();

        long Position { get; set; }

        byte[] Data { get; }

        int Size { get; }
    }
}