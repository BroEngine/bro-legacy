using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingPollRequest : ServiceRequest<MatchmakingPollRequest>
    {
        public readonly IntParam WaitingLimit = new IntParam();
        public readonly IntParam LeavingLimit = new IntParam();
        public readonly IntParam Version = new IntParam();
        public readonly BoolParam IsLocal = new BoolParam();

        public MatchmakingPollRequest() : base(Network.Request.OperationCode.Social.MatchmakingPoll, new MatchmakingChannel() )
        {
            AddParam(WaitingLimit);
            AddParam(LeavingLimit);
            AddParam(Version);
            AddParam(IsLocal);
        }
    }
}

