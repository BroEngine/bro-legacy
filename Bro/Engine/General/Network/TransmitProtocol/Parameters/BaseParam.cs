namespace Bro.Network.TransmitProtocol
{
    public abstract class BaseParam : IPoolCounter, IPoolElement
    {
        public string DebugData;
        public virtual string OwnerData { protected get; set; }

        public readonly bool IsOptional;

        public virtual bool IsInitialized { get; protected set; }

        protected void CheckInitialized()
        {
            if (!IsInitialized)
            {
                Log.Error($"base param :: calling value of no initialized param {GetType()} {OwnerData} {DebugData} {GetHashCode()} ");
            }
        }

        public virtual bool IsValid
        {
            get { return IsOptional || IsInitialized; }
        }

        public virtual bool IsPoolable => true;

        protected BaseParam(bool isOptional)
        {
            IsOptional = isOptional;
        }

        public abstract void Write(IWriter writer);

        public abstract void Read(IReader reader);
        
        public virtual void Cleanup()
        {
            IsInitialized = false;
        }

        public void Retain()
        {
        }

        public void Release()
        {
            NetworkPool.PutBackParam(this);
        }

        public bool IsPoolElement { get; set; }
    }
}
