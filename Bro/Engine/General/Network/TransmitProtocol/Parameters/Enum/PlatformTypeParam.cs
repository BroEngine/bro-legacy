namespace Bro.Network.TransmitProtocol
{
    public class PlatformTypeParam : GenericByteParam<PlatformType>
    {
        public PlatformTypeParam(bool isOptional = false) : base(PlatformType.Undefined, isOptional)
        {
        }

        protected override PlatformType Convert(byte value)
        {
            return (PlatformType) value;
        }

        protected override byte Convert(PlatformType value)
        {
            return (byte) value;
        }
    }
}