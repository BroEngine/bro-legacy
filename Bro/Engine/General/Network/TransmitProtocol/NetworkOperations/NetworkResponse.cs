using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public class NetworkResponse<T> : INetworkResponse where T : class, INetworkResponse, new()
    {
        private readonly byte _operationCode;
        private readonly ByteParam _temporaryIdentifier = new ByteParam();
        private readonly ByteParam _errorCode = new ByteParam(isOptional: true);
        private readonly ParamsCollection _params = new ParamsCollection();
        private readonly IPoolCounter _poolCounter;
        
        NetworkOperationType INetworkOperation.Type => NetworkOperationType.Response;
        byte INetworkOperation.OperationCode => _operationCode;

        byte INetworkResponse.TemporaryIdentifier
        {
            get => _temporaryIdentifier.Value;
            set => _temporaryIdentifier.Value = value;
        }


        byte INetworkResponse.ErrorCode
        {
            get => _errorCode.Value;
            set => _errorCode.Value = value;
        }

        public bool IsHeld { get; set; }
        public bool HasError => _errorCode.IsInitialized && _errorCode.Value != 0;

        short INetworkOperation.OperationCounter { get; set; }

        public bool IsReliable { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsDeferred { get; set; }
        public virtual bool IsPoolable => true;


        void INetworkOperation.Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        void INetworkOperation.Deserialize(IReader reader)
        {
            _params.Read(reader);
        }

        protected void AddParam(BaseParam p)
        {
            _params.AddParam(p);
        }

        protected NetworkResponse(byte operationCode)
        {
#if UNITY_EDITOR
            _params.OwnerData = GetType().ToString();
#endif

            AddParam(_temporaryIdentifier);
            AddParam(_errorCode);

            _operationCode = operationCode;
            IsDeferred = false;
            IsReliable = true;
            IsOrdered = true;

            _poolCounter = new PoolCounter( ()=>{NetworkPool.PutBackOperation(this);});
        }

        ~NetworkResponse()
        {
            if (IsPoolElement
#if UNITY_EDITOR
                && UnityEditor.EditorApplication.isPlaying
#endif
            )
            {
                Bro.Log.Error("Didn't return to pool " + GetType());
            }
        }

        public void Cleanup()
        {
            if (NetworkPool.IsEnabled)
            {
                _params.Cleanup();
            }

        }

        void IPoolCounter.Retain()
        {
            _poolCounter.Retain();
        }

        void IPoolCounter.Release()
        {
            _poolCounter.Release();
        }

        public bool IsPoolElement { get; set; }
    }
}