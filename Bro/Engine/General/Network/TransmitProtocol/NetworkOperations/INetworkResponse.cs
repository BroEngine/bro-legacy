namespace Bro.Network
{
    public interface INetworkResponse : INetworkOperation
    {
        byte TemporaryIdentifier { get; set; }
        bool IsHeld { get; set; }
        byte ErrorCode { get; set; }
    }
}