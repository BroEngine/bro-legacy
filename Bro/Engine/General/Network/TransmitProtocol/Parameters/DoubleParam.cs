using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(double), UniversalParamTypeIndex.Double)]
    public class DoubleParam : BaseParam, IObjectParam
    {
        private double _value;
        private readonly double _minValue;
        private readonly double _maxValue;
        private readonly bool _hasBounds;

        public double Value
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
            get { return Value; }
            set { Value = (double) value; }
        }
        
        System.Type IObjectParam.ValueType => typeof(double);

        public DoubleParam(double minValue, double maxValue, bool isOptional = false) : base(isOptional)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _hasBounds = true;
        }

        public DoubleParam() : this(false)
        {
            
        }

        public DoubleParam(bool isOptional = false) : base(isOptional)
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