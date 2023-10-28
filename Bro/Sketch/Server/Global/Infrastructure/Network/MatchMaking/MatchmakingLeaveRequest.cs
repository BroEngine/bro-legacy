using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingLeaveRequest : ServiceRequest<MatchmakingLeaveRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public MatchmakingLeaveRequest() : base(Network.Request.OperationCode.Social.MatchmakingLeave, new MatchmakingChannel() )
        {
            AddParam(UserId);
        }
    }
}