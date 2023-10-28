using System;
using Bro.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Friends
{
    public class FriendsWindowItemFriend : FriendsWindowItem
    {
        [SerializeField] private Text _labelName; 
        [SerializeField] private Text _labelOfflineTime; 
        [SerializeField] private Text _labelUserData; 
        [SerializeField] private GameObject _objectOnline; 
        [SerializeField] private GameObject _objectOffline;
        [SerializeField] private Button _button;
        
        private Friend _friend;
        private Action<FriendsWindowItemFriend> _buttonCallback;
        private readonly EventObserver<UserStatusChangedEvent> _eventObserver = new EventObserver<UserStatusChangedEvent>();

        public override int UserId
        {
            get { return _friend.UserId; }
        }

        private void Awake()
        {
            _button.onClick.AddListener(OnButton);
        }

        public override void Setup(Friend friend, IClientContext context, Action<FriendsWindowItemFriend> buttonCallback)
        {
            _friend = friend;
            _buttonCallback = buttonCallback;
            _labelName.text = friend.Status.Name;

            if (string.IsNullOrEmpty(_labelName.text))
            {
                _labelName.text = "player_" + friend.UserId;
            }

            UpdateStatus(friend.Status);
        }
        
        private void OnEnable()
        {
            _eventObserver.Subscribe(OnUserStatusChangedEvent);    
        }

        private void OnDisable()
        {
            _eventObserver.Unsubscribe();
        }

        private void OnUserStatusChangedEvent(UserStatusChangedEvent e)
        {
            if (e.Status.UserId == _friend.UserId)
            {
                UpdateStatus(e.Status);
            }
        }
        
        private void UpdateStatus(UserStatus status)
        {
            var isOnline = status.IsOnline();
            
            _objectOnline.SetActive(isOnline);
            _objectOffline.SetActive(!isOnline);
            
            var secondsDelta = (TimeInfo.GlobalTimestamp - status.Timestamp) / 1000;

            var span = TimeSpan.FromSeconds(secondsDelta);
            var daysDelta = span.Days;
            if (daysDelta > 28)
            {
                _labelOfflineTime.text = "never";
            }
            else
            {
                 _labelOfflineTime.text = string.Format("{0}D {1}H {2}M", span.Days, span.Hours, span.Minutes);
            }

            _labelUserData.text = status.Data;
        }

        private void OnButton()
        {
            _buttonCallback?.Invoke(this);
        }
    }
}