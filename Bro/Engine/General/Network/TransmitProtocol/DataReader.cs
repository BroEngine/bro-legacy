namespace Bro.Network.TransmitProtocol
{
    public static class DataReader
    {
        public static IReader GetBinaryReader(byte[] data)
        {
            return new BinaryCacheReader(data);
        }
    }
}