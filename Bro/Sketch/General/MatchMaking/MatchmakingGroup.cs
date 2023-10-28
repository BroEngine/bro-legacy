using System.Collections.Generic;

namespace Bro.Sketch
{
    public class MatchmakingGroup
    {
        public int Token;
        public List<MatchmakingCandidate> Candidates = new List<MatchmakingCandidate>();
    }
}