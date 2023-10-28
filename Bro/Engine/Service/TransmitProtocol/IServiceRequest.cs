using Bro.Service;

namespace Bro.Network.Service
{
    public interface IServiceRequest : IServiceOperation
    {
        byte TemporaryIdentifier { get; set; }
        IServiceChannel ResponseChannel { get; set; }
    }
}