using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(int), UniversalParamTypeIndex.Int)]
    public class IntParam : BaseParam, IIntegerParam, IObjectParam
    {
        private int _value;
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly bool _hasBounds;

        public int Value
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
            set => Value = (int) value;
        }

        System.Type IObjectParam.ValueType => typeof(int);

        public IntParam() : this(false)
        {
        }

        public IntParam(int minValue, int maxValue, bool isOptional = false) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }

        public IntParam(bool isOptional) : base(isOptional)
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