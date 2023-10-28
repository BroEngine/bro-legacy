using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileChannel : IServiceChannel
    {
        public string Path 
        {
            get { return "profile"; }
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