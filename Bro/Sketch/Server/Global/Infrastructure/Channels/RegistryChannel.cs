using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistryChannel : IServiceChannel
    {
        public string Path 
        {
            get { return "registry"; }
        }

        public ServiceChannelType ChannelType
        {
            get { return ServiceChannelType.Queue; }
        }
        
        public ServicePathType PathType
        {
            get { return ServicePathType.Composite; }
        }
    }
}