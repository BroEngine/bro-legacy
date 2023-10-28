using Bro.Network.TransmitProtocol;
using Bro.Service;

namespace Bro.Network.Service
{
    public class ServiceEvent <T> : IServiceEvent where T : IServiceEvent, new()
    {
        public static IServiceEvent Create()
        {
            return new T();
        }

        ServiceOperationType IServiceOperation.Type
        {
            get { return ServiceOperationType.Event; }
        }

        private readonly ParamsCollection _params = new ParamsCollection();
        
        void IServiceOperation.Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        public virtual int ExpirationTimestamp 
        {
            get { return 60000; }
        }
        
        void IServiceOperation.Deserialize(IReader reader)
        {
            _params.Read(reader);
        }
        
        protected void AddParam(BaseParam p)
        {
            _params.AddParam(p);
        }
        
        private readonly byte _operationCode;

        byte IServiceOperation.OperationCode
        {
            get { return _operationCode; }
        }

        public bool IsBroadcast { get; set; }
        
        protected ServiceEvent(byte operationCode, IServiceChannel channel)
        {
            Channel = channel;
            _operationCode = operationCode;
        }
        
        public IServiceChannel Channel { get; set; }
    }
}