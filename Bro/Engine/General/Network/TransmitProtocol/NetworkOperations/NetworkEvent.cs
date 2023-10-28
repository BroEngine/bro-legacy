using Bro.Network.TransmitProtocol;


namespace Bro.Network
{
    public class NetworkEvent<T> : INetworkEvent where T : class, INetworkEvent, new()
    {
        NetworkOperationType INetworkOperation.Type => NetworkOperationType.Event;
        byte INetworkOperation.OperationCode => _operationCode;

        private readonly byte _operationCode;
        public short OperationCounter { get; set; }

        public bool IsReliable { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsDeferred { get; set; }
        public virtual bool IsPoolable => true;

        readonly IPoolCounter _poolCounter;

        private readonly ByteParam _errorCode = new ByteParam(isOptional: true);

        public bool HasError => _errorCode.IsInitialized && _errorCode.Value != Network.ErrorCode.NoError;
        public byte ErrorCode { get => _errorCode.Value; set { _errorCode.Value = value; } }

        void INetworkOperation.Serialize(IWriter writer)
        {
            _params.Write(writer);
        }

        void INetworkOperation.Deserialize(IReader reader)
        {
            try
            {
                _params.Read(reader);
            }
            catch (System.Exception e)
            {
                Bro.Log.Error("Deserialize exception " + e.ToString() + " \n Event = " + _operationCode + " \n data = " + reader.ToString());
            }
        }

        private readonly ParamsCollection _params = new ParamsCollection();

        protected void AddParam(BaseParam p)
        {
            _params.AddParam(p);
        }

        protected NetworkEvent(byte operationCode)
        {
#if UNITY_EDITOR
            _params.OwnerData = GetType().ToString();
#endif

            AddParam(_errorCode);
            _operationCode = operationCode;
            IsDeferred = false;
            IsReliable = true;
            IsOrdered = true;
            _poolCounter = new PoolCounter(() => { NetworkPool.PutBackOperation(this); });
        }

        ~NetworkEvent()
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

        public void Retain()
        {
            _poolCounter.Retain();
        }

        public void Release()
        {
            _poolCounter.Release();
        }

        public bool IsPoolElement { get; set; }
    }
}