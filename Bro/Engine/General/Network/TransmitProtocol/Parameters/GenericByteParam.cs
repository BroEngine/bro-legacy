using System;

namespace Bro.Network.TransmitProtocol
{
    public abstract class GenericByteParam<T> : BaseParam
    {
        private T _value;

        public T Value
        {
            get
            {
                CheckInitialized();
                if (!IsInitialized)
                {
                    throw new Exception("Not initialized");
                }

                return _value;
            }
            set
            {
                _value = value;
                IsInitialized = true;
            }
        }

        protected abstract T Convert(byte value);
        protected abstract byte Convert(T value);
        
        public GenericByteParam(T defaultValue, bool isOptional = false) : base(isOptional)
        {
            _value = defaultValue;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(Convert(_value));
        }

        public override void Read(IReader reader)
        {
            byte buf;
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