namespace Bro.Network.TransmitProtocol
{
    public abstract class GenericShortParam<T> : BaseParam
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

        protected abstract T Convert(short value);
        protected abstract short Convert(T value);

        public GenericShortParam(T defaultValue, bool isOptional = false) : base(isOptional)
        {
            _value = defaultValue;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(Convert(_value));
        }

        public override void Read(IReader reader)
        {
            short buf;
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