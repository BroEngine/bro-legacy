using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationChannel : IServiceChannel
    {
        public string Path 
        {
            get { return "chat"; }
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