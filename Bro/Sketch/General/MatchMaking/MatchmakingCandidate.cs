namespace Bro.Sketch
{
    public class MatchmakingCandidate
    {
        public int UserId;
        public string UserName;
        public string UserData; 
        public int TeamIndex;
        public int GroupId;
        public string Channel;
        public long Timestamp;
        public int Token;
        public int ToleranceMin;
        public int ToleranceMax;
        public int TeamsSize;
        public int TeamsCount;
        public int Version;
        public bool IsLocal;
    }
}