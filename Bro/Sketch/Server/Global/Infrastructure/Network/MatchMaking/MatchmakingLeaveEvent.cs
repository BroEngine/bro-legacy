using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingLeaveEvent : ServiceEvent<MatchmakingLeaveEvent>
    {
        public readonly IntParam UserId = new IntParam();
        public readonly BoolParam IsPolled = new BoolParam();
        
        public MatchmakingLeaveEvent() : base(Event.OperationCode.Social.MatchmakingLeave, null )
        {
            AddParam(UserId);
            AddParam(IsPolled);
        }
    }
}