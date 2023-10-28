using System;

namespace Bro.Network.TransmitProtocol
{
    public class ByteFloatParam : BaseParam, IObjectParam
    {
        private float _value;
        private readonly ByteFloatConverter _converter;

        public float Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                if (value < _converter.MinValue || value > _converter.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value = " + value + "; min = " + _converter.MinValue + " max = "+ _converter.MaxValue);
                }

                _value = value;
                IsInitialized = true;
            }
        }

        object IObjectParam.Value
        {
            get { return Value; }
            set { Value = (float) value; }
        }
        
        System.Type IObjectParam.ValueType => typeof(float);

        public ByteFloatParam(ByteFloatConverter converter, bool isOptional = false) : base(isOptional)
        {
            _converter = converter;
        }


        public override void Write(IWriter writer)
        {
            writer.Write(_converter.ToByte(_value));
        }

        public override void Read(IReader reader)
        {
            byte value;
            reader.Read(out value);
            _value = _converter.ToFloat(value);
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = 0;
            base.Cleanup();
        }
    }
}