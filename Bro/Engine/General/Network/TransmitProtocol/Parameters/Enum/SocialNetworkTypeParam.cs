namespace Bro.Network.TransmitProtocol
{
    public class SocialNetworkTypeParam : GenericByteParam<SocialNetworkType>
    {
        public SocialNetworkTypeParam(bool isOptional = false) : base(SocialNetworkType.Undefined, isOptional)
        {
        }

        protected override SocialNetworkType Convert(byte value)
        {
            return (SocialNetworkType) value;
        }

        protected override byte Convert(SocialNetworkType value)
        {
            return (byte) value;
        }
    }
}