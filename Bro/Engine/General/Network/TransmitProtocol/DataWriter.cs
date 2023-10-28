
namespace Bro.Network.TransmitProtocol
{
    public static class DataWriter
    {
        public static IWriter GetBinaryWriter(int maxSize)
        {
            return new BinaryCacheWriter(maxSize);
        }
    }
}