using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class MatchmakingGroupParam : ParamsCollection
    {
        private readonly IntParam _token = new IntParam(); 
        private readonly ArrayParam<MatchmakingCandidateParam> _candidates = new ArrayParam<MatchmakingCandidateParam>(byte.MaxValue);
        
        public MatchmakingGroupParam() : base(false)
        {
            AddParam(_token);
            AddParam(_candidates);
        }
        
        public MatchmakingGroup Value
        {
            get
            {
                var group = new MatchmakingGroup();
                group.Token = _token.Value;
                foreach (var candidateParam in _candidates.Params)
                {
                    group.Candidates.Add(candidateParam.Value);   
                }
                return group;
            }
            set
            {
                _token.Value = value.Token;
                _candidates.Clear();
                foreach (var candidate in value.Candidates)
                {
                    var candidateParam = NetworkPool.GetParam<MatchmakingCandidateParam>();
                    candidateParam.Value = candidate;
                    _candidates.Add(candidateParam);
                }
            }
        }
    }
}