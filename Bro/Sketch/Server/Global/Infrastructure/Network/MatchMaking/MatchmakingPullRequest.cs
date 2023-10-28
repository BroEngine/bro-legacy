using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingPullRequest : ServiceRequest<MatchmakingPullRequest>
    {
        public readonly ArrayParam<MatchmakingCandidateParam> Candidates = new ArrayParam<MatchmakingCandidateParam>(byte.MaxValue);
        
        public MatchmakingPullRequest() : base(Network.Request.OperationCode.Social.MatchmakingPull, new MatchmakingChannel() )
        {
            AddParam(Candidates);
        }
    }
}