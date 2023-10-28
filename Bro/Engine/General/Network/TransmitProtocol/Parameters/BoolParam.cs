using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(bool), UniversalParamTypeIndex.Bool)]
    
    public class BoolParam : BaseParam, IObjectParam
    {
        private bool _value;

        public bool Value
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

        Type IObjectParam.ValueType => typeof(bool);

        object IObjectParam.Value
        {
            get { return Value; }
            set { Value = (bool) value; }
        }

        public BoolParam() : this(false)
        {

        }
        
        public BoolParam(bool isOptional) : base(isOptional)
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
            _value = default(bool);
            base.Cleanup();
        }
    }
}