using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(byte[]), UniversalParamTypeIndex.ByteArray)]
    public class ByteArrayParam : BaseParam, IObjectParam
    {
        private readonly int _maxSize;
        private byte[] _value;

        private byte[] Value
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

        object IObjectParam.Value
        {
            get => Value;
            set => Value = (byte[]) value;
        }
        
        Type IObjectParam.ValueType => typeof(byte[]);

        public ByteArrayParam() : this(byte.MaxValue,false)
        {

        }

        public ByteArrayParam(int maxSize, bool isOptional) : base(isOptional)
        {
            _maxSize = maxSize;
        }

        public override void Write(IWriter writer)
        {
            int arraySize = _value.Length;
            if (arraySize > _maxSize)
            {
                throw new System.ArgumentException("Wrong array size");
            }

            if (_maxSize <= byte.MaxValue)
            {
                writer.Write((byte) arraySize);
            }
            else if (_maxSize <= short.MaxValue)
            {
                writer.Write((short) arraySize);
            }
            else
            {
                writer.Write((int) arraySize);
            }
            
            writer.Write(_value);
        }

        public override void Read(IReader reader)
        {
            int arraySize = 0;
            if (_maxSize <= byte.MaxValue)
            {
                byte sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else if (_maxSize <= short.MaxValue)
            {
                short sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else
            {
                int sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }

            reader.Read(out _value, arraySize);
            
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = null;
            base.Cleanup();
        }
    }
}