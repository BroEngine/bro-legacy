using Bro.Service;

namespace Bro.Network.TransmitProtocol
{
    public class ServiceChannelParam : ParamsCollection
    {
        private readonly StringParam _channelName = new StringParam();
        private readonly ByteParam _channelType = new ByteParam();
        
        public string ChannelName => _channelName.Value;
        public ServiceChannelType ChannelType => (ServiceChannelType) _channelType.Value;

        public ServiceChannelParam(bool isOptional = false) : base(isOptional)
        {
            AddParam(_channelName);
            AddParam(_channelType);
        }
        
        public ServiceChannelParam() : this(false)
        {
        }

        public ServiceChannelParam(IServiceChannel channel) : this(false)
        {
            Value = channel;
        }
        
        public IServiceChannel Value
        {
            get
            {
                if (IsInitialized)
                {
                    return new BaseServiceChannel(ChannelName, ChannelType, ServicePathType.Direct);
                }

                return null;
            }
            set
            {
                _channelName.Value = value.Path;
                _channelType.Value = (byte) value.ChannelType;
            }
        }
    }
}