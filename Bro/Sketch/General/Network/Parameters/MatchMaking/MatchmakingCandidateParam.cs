using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class MatchmakingCandidateParam : ParamsCollection
    {
        private readonly IntParam _userId = new IntParam();
        private readonly StringParam _userName = new StringParam();
        private readonly StringParam _userData = new StringParam();
        private readonly IntParam _teamIndex = new IntParam();
        private readonly IntParam _groupId = new IntParam();
        private readonly StringParam _channel = new StringParam();
        private readonly LongParam _timestamp = new LongParam();
        private readonly IntParam _token = new IntParam();
        private readonly IntParam _toleranceMin = new IntParam();
        private readonly IntParam _toleranceMax = new IntParam();
        private readonly IntParam _teamsSize = new IntParam();
        private readonly IntParam _teamsCount = new IntParam();
        private readonly IntParam _version = new IntParam();
        private readonly BoolParam _isLocal = new BoolParam();
        
        public MatchmakingCandidateParam() : base(false)
        {
            AddParam(_userId);
            AddParam(_userName);
            AddParam(_userData);
            AddParam(_teamIndex);
            AddParam(_groupId);
            AddParam(_channel);
            AddParam(_timestamp);
            AddParam(_token);
            AddParam(_toleranceMin);
            AddParam(_toleranceMax);
            AddParam(_teamsSize);
            AddParam(_teamsCount);
            AddParam(_version);
            AddParam(_isLocal);
        }
        
        public MatchmakingCandidate Value
        {
            get
            {
                var candidate = new MatchmakingCandidate()
                {
                    UserId = _userId.Value,
                    UserName = _userName.Value,
                    UserData = _userData.Value,
                    TeamIndex = (byte) _teamIndex.Value,
                    GroupId = _groupId.Value,
                    Channel =  _channel.Value,
                    Timestamp = _timestamp.Value,
                    Token = _token.Value,
                    ToleranceMin = _toleranceMin.Value,
                    ToleranceMax = _toleranceMax.Value,
                    TeamsSize = _teamsSize.Value,
                    TeamsCount = _teamsCount.Value,
                    Version = _version.Value,
                    IsLocal = _isLocal.Value
                };

                return candidate;
            }
            set
            {
                _userId.Value = value.UserId;
                _userName.Value = value.UserName;
                _userData.Value = value.UserData;
                _teamIndex.Value = value.TeamIndex;
                _groupId.Value = value.GroupId;
                _channel.Value = value.Channel;
                _timestamp.Value = value.Timestamp;
                _token.Value = value.Token;
                _toleranceMin.Value = value.ToleranceMin;
                _toleranceMax.Value = value.ToleranceMax;
                _teamsSize.Value = value.TeamsSize;
                _teamsCount.Value = value.TeamsCount;
                _version.Value = value.Version;
                _isLocal.Value = value.IsLocal;
            }
        }
    }
}