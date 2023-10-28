using Bro.Network.TransmitProtocol;
using Bro.Service;

namespace Bro.Network.Service
{
    public class ServiceRequest<T> : IServiceRequest where T : IServiceRequest, new()
    {
        public static IServiceRequest Create()
        {
            return new T();
        }

        ServiceOperationType IServiceOperation.Type
        {
            get { return ServiceOperationType.Request; }
        }

        private readonly ParamsCollection _params = new ParamsCollection();
        
        void IServiceOperation.Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        public virtual int ExpirationTimestamp 
        {
            get { return 10000; }
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
       
        private readonly ByteParam _temporaryIdentifier = new ByteParam(isOptional: true);
        private readonly StringParam _responseChannelName = new StringParam(isOptional: true);

        public byte TemporaryIdentifier
        {
            get { return _temporaryIdentifier.Value; }
            set { _temporaryIdentifier.Value = value; }
        }

        public IServiceChannel ResponseChannel {
            get { return new PrivateChannel(_responseChannelName.Value);  }
            set { _responseChannelName.Value = value.Path; }
        }

        protected ServiceRequest(byte operationCode, IServiceChannel channel)
        {
            Channel = channel;
            AddParam(_temporaryIdentifier);
            AddParam(_responseChannelName);
            _operationCode = operationCode;
        }
        
        public IServiceChannel Channel { get; set; }
    }
}