namespace Bro.Network
{
    public interface INetworkRequest : INetworkOperation
    {
        byte TemporaryIdentifier { get; set; }
        bool HasValidParams { get; }
    }
}