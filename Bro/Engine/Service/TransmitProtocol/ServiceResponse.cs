using Bro.Network.TransmitProtocol;
using Bro.Service;

namespace Bro.Network.Service
{
    public class ServiceResponse<T> : IServiceResponse where T : IServiceResponse, new()
    {
        public static IServiceResponse Create()
        {
            return new T();
        }
        
        ServiceOperationType IServiceOperation.Type
        {
            get { return ServiceOperationType.Response; }
        }

        private readonly ParamsCollection _params = new ParamsCollection();
        
        void IServiceOperation.Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        void IServiceOperation.Deserialize(IReader reader)
        {
            _params.Read(reader);
        }

        public virtual int ExpirationTimestamp 
        {
            get { return 10000; }
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
        
        public bool IsHolded { get; set; }
        
        public bool IsBroadcast { get; set; }
        
        private readonly ByteParam _temporaryIdentifier = new ByteParam(isOptional: true);

        public byte TemporaryIdentifier
        {
            get { return _temporaryIdentifier.Value; }
            set { _temporaryIdentifier.Value = value; }
        }
               
        protected ServiceResponse(byte operationCode, IServiceChannel channel)
        {
            Channel = channel;
            AddParam(_temporaryIdentifier);
            _operationCode = operationCode;
        }
        
        public IServiceChannel Channel { get; set; }
    }
}