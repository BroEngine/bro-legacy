using System;
using Bro.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Friends
{
    public class FriendsWindowItemInvite : FriendsWindowItem
    {
        [SerializeField] private FriendsWindowItemFriend _item;
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private Button _buttonAccept;
        [SerializeField] private Button _buttonReject;
        
        private Friend _friend;
        private SessionModule _sessionModule;
        private FriendsModule _friendsModule;
        
        private void Awake()
        {
            _buttonCancel.onClick.AddListener(OnButtonCancel);
            _buttonAccept.onClick.AddListener(OnButtonAccept);
            _buttonReject.onClick.AddListener(OnButtonReject);
        }

        public override int UserId
        {
            get { return _friend.UserId; }
        }
        
        public override void Setup(Friend friend, IClientContext context, Action<FriendsWindowItemFriend> buttonCallback)
        {
            _sessionModule = context.Application.GlobalContext.GetModule<SessionModule>();
            _friendsModule = context.Application.GlobalContext.GetModule<FriendsModule>();
            
            _friend = friend;
            _item.Setup(friend, context, null);

            var isOutgoingInvite = friend.IsOutgoingInvite(_sessionModule.HeroUserId);
            
            _buttonCancel.gameObject.SetActive(isOutgoingInvite);
            _buttonAccept.gameObject.SetActive(!isOutgoingInvite);
            _buttonReject.gameObject.SetActive(!isOutgoingInvite);
        }

        private void OnButtonCancel()
        {
            _friendsModule.RemoveFriend(_friend.UserId);
        }

        private void OnButtonAccept()
        {
            _friendsModule.AcceptFriend(_friend.UserId, null);
        }

        private void OnButtonReject()
        {
            _friendsModule.RemoveFriend(_friend.UserId);
        }
    }
}