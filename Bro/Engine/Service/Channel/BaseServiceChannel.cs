namespace Bro.Service
{
    public class BaseServiceChannel : IServiceChannel
    {
        public string Path { get; }
        public ServiceChannelType ChannelType { get; }
        public ServicePathType PathType { get; }
        
        public BaseServiceChannel(string path, ServiceChannelType channelType, ServicePathType pathType)
        {
            Path = path;
            ChannelType = channelType;
            PathType = pathType;
        }
    }
}