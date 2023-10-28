using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class MatchmakingPollResponse : ServiceResponse<MatchmakingPollResponse>
    {
        private readonly ArrayParam<MatchmakingGroupParam> _groups = new ArrayParam<MatchmakingGroupParam>(byte.MaxValue);
        
        public MatchmakingPollResponse() : base(Network.Request.OperationCode.Social.MatchmakingPoll, new MatchmakingChannel())
        {
            AddParam(_groups);
        }

        public List<MatchmakingGroup> Groups
        {
            get
            {
                var groups = new List<MatchmakingGroup>();
                foreach (var groupParam in _groups.Params)
                {
                    groups.Add(groupParam.Value);
                }
                return groups;
            }
        }
    }
}

