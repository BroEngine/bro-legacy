namespace Bro.Network.TransmitProtocol
{
    public interface IParam
    {
        void Write(IWriter writer);
        void Read(IReader reader);
    }
}