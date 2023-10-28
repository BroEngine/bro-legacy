using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class FriendsModule : IClientContextModule
    {
        private const long AvailabilityCheckPeriod = 5000L;
        
        protected IClientContext _context;
        
        private readonly Stopwatch _updateTimer = new Stopwatch();
        private FriendsStorage _storage;
        private IntentDataModule _intentDataModule;
        private UserStatusModule _userStatusModule;
        private SessionModule _sessionModule;
        private NetworkEngine _networkEngine;
        private IDisposable _onUpdateHandler;
        
        private bool _isAvailable;
        
        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();

        public List<Friend> Friends => _storage.Friends;
        public bool IsReady => _storage.IsReady;
        
        public virtual void Initialize(IClientContext context)
        {
            _context = context;
            _networkEngine = _context.GetNetworkEngine();
            _intentDataModule = _context.GetModule<IntentDataModule>();
            _userStatusModule = _context.GetModule<UserStatusModule>();
            _sessionModule = _context.GetModule<SessionModule>();
            _storage = new FriendsStorage(_userStatusModule, _sessionModule);
        }

        public IEnumerator Load()
        {
            var engine = _context.GetNetworkEngine();
            _context.AddDisposable(new NetworkEventObserver<Network.FriendsStatusEvent>(OnFriendsStatusEvent, engine));
            _context.AddDisposable(new EventObserver<UserStatusChangedEvent>(OnUserStatusChangedEvent));
            _context.AddDisposable(new EventObserver<ApplicationFocusEvent>(OnApplicationFocusEvent));
            _networkEngine.OnStatusChanged += OnServerStatusChanged;
            _onUpdateHandler = _context.Scheduler.ScheduleUpdate(OnUpdate);
            
            yield return null;
        }

        public IEnumerator Unload()
        {
            _updateTimer.Stop();
            _onUpdateHandler.Dispose();
            _onUpdateHandler = null;
            _networkEngine.OnStatusChanged -= OnServerStatusChanged;
            yield return null;
        }
        
        private void SetServiceAvailability(bool value)
        {
            if (_isAvailable != value)
            {
                _isAvailable = value;
                if (value)
                {
                    CheckIntentFriends();
                }
            }
        }
        
        private void OnFriendsStatusEvent(Network.FriendsStatusEvent statusEvent)
        {
            var friend = statusEvent.Friend.Value;
            var friendUserId = friend.UserId;
            var state = friend.State;
            
            switch (state)
            {
                case FriendState.Pending:
                    if (_storage.IsInviteLimitReached(true))
                    {
                        RemoveFriend(friend.UserId);
                    }
                    else
                    {
                        _storage.Add(friend);
                    }
                    break;
                case FriendState.Friends:
                    
                    if (!IsFriend(friendUserId, true))
                    {
                        _storage.Add(friend);
                        new FriendsStateEvent(state, friend).Launch();
                    }
                    else
                    {
                        foreach (var existingFriend in _storage.Friends)
                        {
                            if (existingFriend.UserId == friendUserId)
                            {
                                existingFriend.State = state;
                                new FriendsStateEvent(state, friend).Launch();
                            }
                        }
                    }

                    new FriendsListChangedEvent(_storage.Friends).Launch();
                    break;
                case FriendState.Removed:
                    _storage.Remove(friendUserId, true);
                    break;
            }
        }

        private void OnServerStatusChanged(NetworkStatus status, int descriptionCode)
        {
            if (status == NetworkStatus.Disconnected)
            {
                SetServiceAvailability(false);
            }

            if (status == NetworkStatus.Connected)
            {
                _updateTimer.Restart();
            }
        }

        private void OnUserStatusChangedEvent(UserStatusChangedEvent statusChangedEvent)
        {
            _storage.UpdateStatus(statusChangedEvent.Status);
        }

        private void OnApplicationFocusEvent(ApplicationFocusEvent applicationFocusEvent)
        {
            if (applicationFocusEvent.IsFocus)
            {
                CheckIntentFriends();
            }
        }

        private void SetFriendStatus(int userId, FriendState state, Action<bool> callback)
        {
            var task = new FriendsSetTask(_context, userId, state);
            task.OnComplete += (t) => { callback?.Invoke(true); };
            task.OnFail += (t) => { callback?.Invoke(false); };
            task.Launch(_context);
        }
        
        public bool IsFriend(int userId, bool canBePending = false)
        {
            if (canBePending)
            {
                return _storage.Friends.Any(friend => friend.UserId == userId);
            }

            return _storage.Friends.Any(friend => friend.UserId == userId && friend.State == FriendState.Friends);
        }
        
        public Friend GetFriend(int userId)
        {
            return _storage.Friends.Find(friend => friend.UserId == userId);
        }
        
        public virtual void AddFriend(int userId, Action<bool> callback)
        {
            SetFriendStatus(userId, FriendState.Pending, callback);   
        }

        public void AcceptFriend(int userId, Action<bool> callback)
        {
            SetFriendStatus(userId, FriendState.Friends, callback);
        }
        
        public void RemoveFriend(int userId)
        {
            var task = new FriendsDeleteTask(_context, userId);
            task.Launch(_context);
            _storage.Remove(userId, invoke: true);
        }
        
        public bool CanInviteFriend()
        {
            if (_storage.IsFriendsLimitReached())
            {
                return false;
            }

            if (_storage.IsInviteLimitReached(false))
            {
                return false;
            }

            return true;
        }

        public void UpdateList()
        {
            var task = new FriendsGetTask(_context);
            task.OnComplete += (t) =>
            {
                _storage.Update(task.Friends);
                SetServiceAvailability(true);
            };
            task.OnFail += (t) =>
            {
                SetServiceAvailability(false);
            };
            task.Launch(_context);
        }
        
        private void OnUpdate(float delta)
        {
            var isConnected = _networkEngine.IsConnected();
            if (isConnected && !_isAvailable && _updateTimer.ElapsedMilliseconds > AvailabilityCheckPeriod)
            {
                UpdateList();
                _updateTimer.Restart();
            }
        }
        
        private void CheckIntentFriends()
        {
            if (!_isAvailable)
            {
                return;
            }

            var hash = _intentDataModule.GetFriendInvitingHash();

            if (!string.IsNullOrEmpty(hash))
            {
                _intentDataModule.SetHashProcessed(hash);

                var friendId = IdentificatorHash.Decode(hash);
                
                Log.Info("friend :: intent friend id = " + friendId);

                if (friendId != 0)
                {
                    var heroUserId = _sessionModule.HeroUserId;

                    if (heroUserId == friendId)
                    {
                        return;
                    }

                    AddFriend(friendId, (addResult) =>
                    {
                        Log.Info("friend :: intent add result friend id = " + friendId + " result = " + addResult);

                        AcceptFriend(friendId, (acceptResult) =>
                        {
                            Log.Info("friend :: intent accept friend id = " + friendId + " result = " + acceptResult);
                            UpdateList();
                        });
                    });
                }
            }
        }
    }
}