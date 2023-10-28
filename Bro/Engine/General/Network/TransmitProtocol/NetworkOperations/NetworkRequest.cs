using Bro.Network.TransmitProtocol;

namespace Bro.Network
{
    public class NetworkRequest<T> : INetworkRequest where T : class, INetworkRequest, new()
    {
        private readonly ByteParam _temporaryIdentifier = new ByteParam();

        public byte TemporaryIdentifier
        {
            get => _temporaryIdentifier.Value;
            set => _temporaryIdentifier.Value = value;
        }

        public bool HasValidParams => _params.IsValid;

        public NetworkOperationType Type => NetworkOperationType.Request;
        public byte OperationCode { get; }

        public short OperationCounter { get; set; }

        public bool IsReliable { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsDeferred { get; set; }
        public virtual bool IsPoolable { get { return true; } }
        
        private readonly IPoolCounter _poolCounter;

        public void Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        public void Deserialize(IReader reader)
        {
            _params.Read(reader);
        }

        private readonly ParamsCollection _params = new ParamsCollection();

        protected void AddParam(BaseParam p)
        {
            _params.AddParam(p);
        }

        protected NetworkRequest(byte operationCode)
        {
            #if UNITY_EDITOR
            _params.OwnerData = GetType().ToString();
            #endif

            AddParam(_temporaryIdentifier);

            OperationCode = operationCode;
            IsDeferred = false;
            IsReliable = true;
            IsOrdered = true;

            _poolCounter = new PoolCounter(()=>{NetworkPool.PutBackOperation(this);});
        }

        ~NetworkRequest()
        {
            if (IsPoolElement
#if UNITY_EDITOR
                && UnityEditor.EditorApplication.isPlaying
#endif
            )
            {
                Bro.Log.Error($"Didn't returned to pool {GetType()} {GetHashCode()} {((PoolCounter)_poolCounter).Counter }");
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