using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingPullResponse : ServiceResponse<MatchmakingPullResponse>
    {
        public readonly BoolParam IsSuccessful = new BoolParam();
        
        public MatchmakingPullResponse() : base(Network.Request.OperationCode.Social.MatchmakingPull, new MatchmakingChannel())
        {
            AddParam(IsSuccessful);
        }
    }
}