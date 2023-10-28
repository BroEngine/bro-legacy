using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public interface INetworkOperation : IPoolCounter, IPoolElement
    {
        NetworkOperationType Type { get; }
        byte OperationCode { get; }
        short OperationCounter { get; set; }
        bool IsReliable { get; }
        bool IsOrdered { get; }
        bool IsDeferred { get; }
        void Cleanup();

        void Serialize(IWriter writer);
        void Deserialize(IReader reader);
    }
}