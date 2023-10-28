using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(byte), UniversalParamTypeIndex.Byte)]
    public class ByteParam : BaseParam, IIntegerParam, IObjectParam
    {
        private byte _value;
        private readonly byte _minValue;
        private readonly byte _maxValue;
        private readonly bool _hasBounds;

        int IIntegerParam.Value
        {
            get => Value;
            set
            {
                if (value <= byte.MaxValue && value >= byte.MinValue)
                {
                    Value = (byte) value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"byte param value out of range, value = {value} range = [{byte.MinValue}...{byte.MaxValue}]");
                }
            }
        }
        
        System.Type IObjectParam.ValueType => typeof(byte);

        public byte Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                if (_hasBounds && (_value < _minValue || _value > _maxValue))
                {
                    throw new ArgumentOutOfRangeException($"byte param value out of range, value = {value} range = [{_minValue}...{_maxValue}]");
                }

                _value = value;
                IsInitialized = true;
            }
        }

        object IObjectParam.Value
        {
            get => Value;
            set => Value = (byte) value;
        }

        public ByteParam() : this(false)
        {
            _hasBounds = false;
        }
        
        public ByteParam(byte minValue, byte maxValue, bool isOptional) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }
        
        public ByteParam(bool isOptional) : base(isOptional)
        {
            _hasBounds = false;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(_value);
        }

        public override void Read(IReader reader)
        {
            reader.Read(out _value);
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = 0;
            base.Cleanup();
        }
    }
}