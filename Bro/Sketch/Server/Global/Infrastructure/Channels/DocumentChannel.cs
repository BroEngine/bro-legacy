using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentChannel : IServiceChannel
    {
        public string Path 
        {
            get { return "document"; }
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