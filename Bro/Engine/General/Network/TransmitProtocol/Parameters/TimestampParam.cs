namespace Bro.Network.TransmitProtocol
{
    public class TimestampParam : BaseParam
    {
        private long _value;

        public long Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
        }


        public override bool IsInitialized { get => true; protected set { } }

        public TimestampParam(bool isOptional = false) : base(isOptional)
        {
        }

        public override void Read(IReader reader)
        {
            reader.Read(out _value);
        }

        public override void Write(IWriter writer)
        {
            writer.Write(TimeInfo.LocalTimestamp);
        }
    }
}