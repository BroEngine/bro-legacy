using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public class LetsEncryptOperation : INetworkOperation
    {
        public enum EncryptionStep
        {
            PublicPart,
            SecretPart,
            HandShake,
            Discard
        }

        public EncryptionStep Step { get; protected set; }
        public long Value { get; protected set; }

        public LetsEncryptOperation()
        {
        }

        public LetsEncryptOperation(EncryptionStep step, long value)
        {
            Step = step;
            Value = value;
        }

        public byte OperationCode
        {
            get { return 0; }
        }

        public short OperationCounter { get; set; }

        public NetworkOperationType Type
        {
            get { return NetworkOperationType.Encryption; }
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
            writer.Write((byte) Step);
            writer.Write((long) Value);
        }

        public void Deserialize(IReader reader)
        {
            byte step;
            long value;

            reader.Read(out step);
            reader.Read(out value);

            Step = (EncryptionStep) step;
            Value = value;
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