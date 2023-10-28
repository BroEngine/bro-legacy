namespace Bro.Service
{
    public interface IServiceChannel
    {
        string Path { get; }
        ServiceChannelType ChannelType { get; }
        ServicePathType PathType { get; }
    }
}