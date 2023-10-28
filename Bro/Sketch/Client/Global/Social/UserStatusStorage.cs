using System.Collections.Generic;
using System.Diagnostics;

namespace Bro.Sketch.Client
{
    public class UserStatusStorage
    {
        private readonly Stopwatch _timer;
        private readonly Dictionary<int, UserStatus> _statuses = new Dictionary<int, UserStatus>();
            
        public UserStatusStorage()
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public UserStatus GetUserStatus(int userId)
        {
            return _statuses.ContainsKey(userId) ? _statuses[userId] : null;
        }
        
        public void OnUpdate()
        {
            if (_timer.ElapsedMilliseconds > 1000L)
            {
                foreach (var statusPair in _statuses)
                {
                    var status = statusPair.Value;
                    var oldOnlineStatus = status.State;
                    var onlineStatus = Validate(status);

                    if (oldOnlineStatus != onlineStatus)
                    {
                        status.State = onlineStatus;
                        new UserStatusChangedEvent(status).Launch();
                    }
                }
                
                _timer.Restart();
            }
        }
        
        public void RegisterAndValidate(ref UserStatus status) 
        {
            status.State = Validate(status);
            
            if (!_statuses.ContainsKey(status.UserId))
            {
                _statuses.Add( status.UserId, status );
                new UserStatusChangedEvent(status).Launch();
            }
            else
            {
                var oldTimestamp = _statuses[status.UserId].Timestamp;
                if (status.Timestamp > oldTimestamp)
                {
                    _statuses[status.UserId].State = status.State;
                    _statuses[status.UserId] = status;
                    new UserStatusChangedEvent(status).Launch();
                }
                else
                {
                    status = _statuses[status.UserId];
                }
            }
        }
        
        public byte GetUserState(int userId)
        {
            if (_statuses.ContainsKey(userId))
            {
                return _statuses[userId].State;
            }
            return 0;
        }

        private byte Validate(UserStatus status)
        {
            // todo
            return status.State;
        }
    }
}