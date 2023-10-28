using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public class PingOperation : INetworkOperation
    {
        public byte OperationCode
        {
            get { return 0; }
        }

        public short OperationCounter { get; set; }

        public NetworkOperationType Type
        {
            get { return NetworkOperationType.Ping; }
        }

        public bool IsReliable
        {
            get { return true; }
        }

        public bool IsOrdered
        {
            get { return true; }
        }

        public bool IsDeferred
        {
            get { return false; }
        }

        public bool IsPoolable
        {
            get { return false; }
        }
        
        public void Cleanup()
        {
            
        }

        public void Serialize(IWriter writer)
        {
        }

        public void Deserialize(IReader reader)
        {
        }

        public void Retain()
        {
        }

        public void Release()
        {
        }

        public bool IsPoolElement { get; set; }
    }
}