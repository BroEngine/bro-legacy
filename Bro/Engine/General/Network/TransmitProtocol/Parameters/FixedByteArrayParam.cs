namespace Bro.Network.TransmitProtocol
{
    public class FixedByteArrayParam : BaseParam
    {
        private readonly int _size;
        private byte[] _value;

        public byte[] Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                if (value.Length != _size)
                {
                }
                else
                {
                    _value = value;
                }

                IsInitialized = true;
            }
        }

        public FixedByteArrayParam(int size, bool isOptional = false) : base(isOptional)
        {
            _size = size;
        }

        public override void Write(IWriter writer)
        {
            writer.Write(_value);
        }

        public override void Read(IReader reader)
        {
            reader.Read(out _value, _size);
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = new byte[0];
            base.Cleanup();
        }
    }
}