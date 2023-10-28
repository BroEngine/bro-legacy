namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(string), UniversalParamTypeIndex.String)]
    public class StringParam : BaseParam, IObjectParam
    {
        private string _value;

        public string Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                _value = value;
                if (value != null)
                {
                    IsInitialized = true;
                }
                else
                {
                    // todo think how to pass null, or all fine
                    _value = string.Empty;
                    IsInitialized = true;
                }
            }
        }

        public string ValueOrDefault
        {
            get
            {
                if (IsInitialized)
                {
                    return Value;
                }

                return string.Empty; /* not null */
            }
        }
        
        object IObjectParam.Value
        {
            get { return Value; }
            set { Value = (string) value; }
        }
        
        System.Type IObjectParam.ValueType => typeof(string);

        public StringParam() : this(false)
        {
        }

        public StringParam(bool isOptional = false) : base(isOptional)
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
            _value = string.Empty;
            base.Cleanup();
        }
    }
}