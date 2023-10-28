namespace Bro.Network.TransmitProtocol
{
    public interface IObjectParam
    {
        object Value { get; set; }
        System.Type ValueType { get; }
    }
}