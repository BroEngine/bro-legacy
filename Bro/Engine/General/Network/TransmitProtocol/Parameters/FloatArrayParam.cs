namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(float[]), UniversalParamTypeIndex.FloatArray)]
    public class FloatArrayParam : BaseParam, IObjectParam
    {
        private readonly int _maxSize;
        private float[] _value;

        public float[] Value
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

        object IObjectParam.Value
        {
            get { return Value; }
            set { Value = (float[]) value; }
        }
        
        System.Type IObjectParam.ValueType => typeof(float[]);

        public FloatArrayParam() : this(byte.MaxValue, false)
        {

        }
        
        public FloatArrayParam(int maxSize, bool isOptional) : base(isOptional)
        {
            _maxSize = maxSize;
        }

        public override void Write(IWriter writer)
        {
            int arraySize = _value.Length;
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

            _value = new float[arraySize];
            short readBuf;
            for (int i = 0, max = _value.Length; i < max; ++i)
            {
                reader.Read(out readBuf);
                _value[i] = readBuf;
            }

            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = new float[0];
            base.Cleanup();
        }
    }
}