namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(UnknownObject), UniversalParamTypeIndex.UnknownObject)]
    public class UnknownObjectParam : BaseParam, IObjectParam
    {
        private UnknownObject _value;
        private readonly int _size;

        public UnknownObject Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set => _value = value;
        }

        object IObjectParam.Value { get => _value; set => _value = (UnknownObject) value; }
        
        System.Type IObjectParam.ValueType => typeof(UnknownObject);

        public UnknownObjectParam() : this(byte.MaxValue, false)
        {
        }

        public UnknownObjectParam(int size, bool isOptional = false) : base(isOptional)
        {
            _size = size;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(_value.Data);
        }

        public override void Read(IReader reader)
        {
            if (_value == null)
            {
                _value = new UnknownObject();
            }

            reader.Read(out _value.Data, _size);
        }

        public override void Cleanup()
        {
            _value.Data = new byte[0];
            base.Cleanup();
        }
    }
}