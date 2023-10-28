namespace Bro.Service
{
    public class PrivateChannel : IServiceChannel
    {
        public string Path { get; }
        public ServiceChannelType ChannelType { get; }
        public ServicePathType PathType => ServicePathType.Direct;
        
        public PrivateChannel()
        {
            Path = "p_" + ( TimeInfo.GlobalTimestamp + Random.Instance.Range(100000, 999999) ).ToString();
            ChannelType = ServiceChannelType.Queue;
        }
        
        public PrivateChannel(string name)
        {
            Path = name;
            ChannelType = ServiceChannelType.Queue;
        }
    }
}