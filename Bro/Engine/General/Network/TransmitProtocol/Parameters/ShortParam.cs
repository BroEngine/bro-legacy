using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(short), UniversalParamTypeIndex.Short)]
    public class ShortParam : BaseParam, IIntegerParam, IObjectParam
    {
        private short _value;
        private readonly short _minValue;
        private readonly short _maxValue;
        private readonly bool _hasBounds;

        int IIntegerParam.Value
        {
            get { return Value; }
            set { Value = (short) value; }
        }

        public short Value
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
                    throw new ArgumentOutOfRangeException();
                }

                _value = value;
                IsInitialized = true;
            }
        }

        object IObjectParam.Value
        {
            get => Value;
            set => Value = (short) value;
        }
        
        System.Type IObjectParam.ValueType => typeof(short);

        public ShortParam(short minValue, short maxValue, bool isOptional = false) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }

        public ShortParam() : base(false)
        {
            _hasBounds = false;
        }
        
        public ShortParam(bool isOptional = false) : base(isOptional)
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