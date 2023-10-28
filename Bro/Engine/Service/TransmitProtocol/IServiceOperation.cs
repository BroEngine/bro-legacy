using Bro.Network.TransmitProtocol;
using Bro.Service;

namespace Bro.Network.Service
{
    public interface IServiceOperation
    {
        ServiceOperationType Type { get; }
        IServiceChannel Channel { get; set; }
        byte OperationCode { get; }
        void Serialize(IWriter writer);
        void Deserialize(IReader reader);
        int ExpirationTimestamp { get; }
    }
}