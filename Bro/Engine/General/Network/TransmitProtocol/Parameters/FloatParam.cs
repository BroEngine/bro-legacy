using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(float), UniversalParamTypeIndex.Float)]
    public class FloatParam : BaseParam, IObjectParam
    {
        private float _value;
        private readonly float _minValue;
        private readonly float _maxValue;
        private readonly bool _hasBounds;

        public float Value
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
            set => Value = (float) value;
        }
        
        Type IObjectParam.ValueType => typeof(float);

        public FloatParam(float minValue, float maxValue, bool isOptional = false) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }

        public FloatParam(bool isOptional = false) : base(isOptional)
        {
            _hasBounds = false;
        }

        public FloatParam() : this(false)
        {
            
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