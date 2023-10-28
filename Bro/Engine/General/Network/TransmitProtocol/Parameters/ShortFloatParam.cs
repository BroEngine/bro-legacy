using System;

namespace Bro.Network.TransmitProtocol
{
    public class ShortFloatParam : BaseParam
    {
        private float _value;
        private readonly ShortFloatConverter _converter;

        public float Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                if (!_converter.IsValid(value))
                {
                    Bro.Log.Error("value is invalid: cur = " + value + " min = " +  _converter.MinValue + " max = " +  _converter.MaxValue);
                }

                _value = value;
                IsInitialized = true;
            }
        }

        public ShortFloatParam(ShortFloatConverter converter, bool isOptional = false) : base(isOptional)
        {
            _converter = converter;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(_converter.ToShort(_value));
        }

        public override void Read(IReader reader)
        {
            short value;
            reader.Read(out value);
            _value = _converter.ToFloat(value);
            IsInitialized = true;


            // todo make buity, zero value
            if (Math.Abs(_value) < 0.001f)
            {
                _value = 0.0f;
            }
        }

        public override void Cleanup()
        {
            _value = 0;
            base.Cleanup();
        }
    }
}