using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingChannel : IServiceChannel
    {
        public string Path 
        {
            get { return "matchmaking"; }
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

