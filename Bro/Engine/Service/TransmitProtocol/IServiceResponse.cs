namespace Bro.Network.Service
{
    public interface IServiceResponse : IServiceOperation
    {
        byte TemporaryIdentifier { get; set; }
        bool IsHolded { get; set; }
    }
}