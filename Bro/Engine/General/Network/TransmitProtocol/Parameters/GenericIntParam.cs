namespace Bro.Network.TransmitProtocol
{
    public abstract class GenericIntParam<T> : BaseParam
    {
        private T _value;

        public T Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                _value = value;
                IsInitialized = true;
            }
        }

        protected abstract T Convert(int value);
        protected abstract int Convert(T value);

        public GenericIntParam(T defaulValue, bool isOptional = false) : base(isOptional)
        {
            _value = defaulValue;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(Convert(_value));
        }

        public override void Read(IReader reader)
        {
            int buf;
            reader.Read(out buf);
            Value = Convert(buf);
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = default(T);
            base.Cleanup();
        }
    }
}