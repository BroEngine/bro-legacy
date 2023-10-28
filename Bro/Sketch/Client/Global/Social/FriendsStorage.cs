using System.Collections.Generic;
using System.Linq;

namespace Bro.Sketch.Client
{
    public class FriendsStorage
    {
        private const int FriendsLimit = 100;
        private const int IncomingInvitesLimit = 5;
        private const int OutgoingInvitesLimit = 5;
        
        private readonly List<Friend> _friends = new List<Friend>();
        private readonly UserStatusModule _userStatusModule;
        private readonly SessionModule _sessionModule;
        
        public List<Friend> Friends => _friends;
        public bool IsReady { get; private set; }

        public FriendsStorage(UserStatusModule userStatusModule, SessionModule sessionModule)
        {
            _sessionModule = sessionModule;
            _userStatusModule = userStatusModule;
        }

        public void Add(Friend friend)
        {
            Remove(friend.UserId, invoke: false);
            
            _userStatusModule.RegisterAndValidate(ref friend.Status);
            _friends.Add(friend);
            
            new FriendsListChangedEvent(_friends).Launch();
        }

        public void Remove(int userId, bool invoke)
        {
            for (var i = _friends.Count - 1; i >= 0; --i)
            {
                if (_friends[i].UserId == userId)
                {
                    _friends.RemoveAt(i);
                }
            }

            if (invoke)
            {
                new FriendsListChangedEvent(_friends).Launch();
            }
        }

        public void Update(List<Friend> friends)
        {
            _friends.Clear();
            foreach (var friend in friends)
            {
                _userStatusModule.RegisterAndValidate(ref friend.Status);
                _friends.Add(friend);
            }
            IsReady = true;
            
            new FriendsListChangedEvent(_friends).Launch();
        }

        public bool IsInviteLimitReached(bool incoming)
        {
            var heroUserId = _sessionModule.HeroUserId;
            var limit = incoming ? IncomingInvitesLimit : OutgoingInvitesLimit;
            var count = incoming
                ? _friends.Count(friend => friend.State == FriendState.Pending && !friend.IsOutgoingInvite(heroUserId))
                : _friends.Count(friend => friend.State == FriendState.Pending && friend.IsOutgoingInvite(heroUserId));

            return count >= limit;
        }
        
        public bool IsFriendsLimitReached()
        {
            return Friends.Count >= FriendsLimit;
        }

        public void UpdateStatus(UserStatus status)
        {
            var userId = status.UserId;
            foreach (var friend in _friends)
            {
                if (friend.UserId == userId)
                {
                    friend.Status = status;
                    break;
                }
            }
        }
    }
}