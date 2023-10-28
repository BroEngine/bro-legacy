namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(string[]), UniversalParamTypeIndex.StringArray)]
    public class StringArrayParam : BaseParam, IObjectParam
    {
        private readonly int _maxSize;
        private string[] _value;

        public string[] Value
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

        object IObjectParam.Value { get => Value; set => Value = (string[]) value; }
        System.Type IObjectParam.ValueType => typeof(string[]);

        public StringArrayParam() : this(byte.MaxValue)
        {
        }

        public StringArrayParam(int maxSize, bool isOptional = false) : base(isOptional)
        {
            _maxSize = maxSize;
        }

        public override void Write(IWriter writer)
        {
            var arraySize = _value.Length;
            if (arraySize > _maxSize)
            {
                throw new System.ArgumentException("Wrong array size");
            }

            if (_maxSize <= byte.MaxValue)
            {
                writer.Write((byte) arraySize);
            }
            else if (_maxSize <= short.MaxValue)
            {
                writer.Write((short) arraySize);
            }
            else
            {
                writer.Write((int) arraySize);
            }

            for (int i = 0, max = _value.Length; i < max; ++i)
            {
                writer.Write(_value[i]);
            }
        }

        public override void Read(IReader reader)
        {
            int arraySize = 0;
            if (_maxSize <= byte.MaxValue)
            {
                byte sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else if (_maxSize <= short.MaxValue)
            {
                short sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }
            else
            {
                int sizeValue;
                reader.Read(out sizeValue);
                arraySize = sizeValue;
            }

            _value = new string[arraySize];
            string readBuf;
            for (int i = 0, max = _value.Length; i < max; ++i)
            {
                reader.Read(out readBuf);
                _value[i] = readBuf;
            }

            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = new string[0];
            base.Cleanup();
        }
    }
}