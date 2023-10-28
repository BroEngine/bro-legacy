using System;
using System.Collections.Generic;
using Bro.Toolbox.Client.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Bro.Client;

namespace Bro.Sketch.Client.Friends
{
    /* НЕ ТРОГАТЬ И НЕ ИЗМЕНЯТЬ !!! */
    [Window()]
    public class FriendsWindow : Window
    {
        private enum Tab
        {
            Friends,
            Invites
        }

        [SerializeField] private GameObject _prefabFriend;
        [SerializeField] private GameObject _prefabInvite;
        [SerializeField] private GameObject _prefabControl;
        
        [SerializeField] private Button _buttonClose;
        [SerializeField] private Button _buttonTabFriends;
        [SerializeField] private Button _buttonTabInvites;
        [SerializeField] private Button _buttonAddFriend;
        [SerializeField] private Button _buttonShareInvite;
        [SerializeField] private Button _buttonJoinCommunity;

        [SerializeField] private InputField _inputFieldFriendId;

        [SerializeField] private Text _labelUserId;
        [SerializeField] private Text _labelCountFriendsAll;
        [SerializeField] private Text _labelCountFriendsOnline;
        [SerializeField] private Text _labelCountIncomingInvites;
        
        [SerializeField] private Transform _containerScrollFriends;
        [SerializeField] private Transform _containerScrollOutgoingInvites;
        [SerializeField] private Transform _containerScrollIncomingInvites;

        [SerializeField] private GameObject _objectNoInvites;
        [SerializeField] private GameObject _objectTabFriendsSelected;
        [SerializeField] private GameObject _objectTabInvitesSelected;
        [SerializeField] private GameObject _objectTabFriends;
        [SerializeField] private GameObject _objectTabInvites;
        [SerializeField] private GameObject _objectCounterIncomingInvites;

        private FriendsWindowControlPopup _controlPopup;
        
        private Tab _currentTab;
        private UIModule _uiModule;
        private SessionModule _sessionModule;
        private FriendsModule _friendsModule;
        private IntentDataModule _intentDataModule;
        private readonly EventObserver<FriendsListChangedEvent> _friendsListChangedObserver = new EventObserver<FriendsListChangedEvent>();
        
        private readonly List<FriendsWindowItem> _itemsFriends = new List<FriendsWindowItem>();
        private readonly List<FriendsWindowItem> _itemsOutgoingInvites = new List<FriendsWindowItem>();
        private readonly List<FriendsWindowItem> _itemsIncomingInvites = new List<FriendsWindowItem>();
        
        private void Awake()
        {
            _buttonClose.onClick.AddListener(OnButtonClose);
            _buttonTabFriends.onClick.AddListener(OnButtonTabFriends);
            _buttonTabInvites.onClick.AddListener(OnButtonTabInvites);
            _buttonAddFriend.onClick.AddListener(OnButtonAddFriend);
            _buttonShareInvite.onClick.AddListener(OnButtonShareInvites);
            _buttonJoinCommunity.onClick.AddListener(OnButtonJoinCommunity);
            _inputFieldFriendId.onValueChanged.AddListener(OnInputValueChanged);
            _controlPopup = Instantiate(_prefabControl, transform).GetComponent<FriendsWindowControlPopup>();
            OnInputValueChanged(_inputFieldFriendId.text);
        }

        private void OnEnable()
        {
            _friendsListChangedObserver.Subscribe(OnFriendsListChangedEvent);    
        }

        private void OnDisable()
        {
            _friendsListChangedObserver.Unsubscribe();
        }
        
        protected override void OnShow(IWindowArgs args)
        {
            _uiModule = Context.GetModule<UIModule>();
            _friendsModule = Context.Application.GlobalContext.GetModule<FriendsModule>();
            _intentDataModule = Context.Application.GlobalContext.GetModule<IntentDataModule>();
            _sessionModule = Context.Application.GlobalContext.GetModule<SessionModule>();
            
            OnButtonTabFriends();
        }

        private void OnFriendsListChangedEvent(FriendsListChangedEvent e)
        {
            UpdateContent();
        }

        private void OnButtonClose()
        {
            _uiModule.DirectlyHide<FriendsWindow>();
        }
        
        private void OnButtonTabFriends()
        {
            _currentTab = Tab.Friends;
            UpdateContent();
        }

        private void OnButtonTabInvites()
        {
            _currentTab = Tab.Invites;
            UpdateContent();
        }
        
        private void OnButtonAddFriend()
        {
            var text = _inputFieldFriendId.text;
            _inputFieldFriendId.text = string.Empty;

            var userId = 0;
          
            if (string.IsNullOrEmpty(text.Trim()))
            {
                return;
            }

            if (text.Length == 1)
            {
                userId = Int32.Parse(text);
            }
            else
            {
                try
                {
                    userId = IdentificatorHash.Decode(text);
                }
                catch (Exception)
                {
                    Log.Error("text field invalid");
                    return;
                } 
            }
        
            if (userId == 0)
            {
                return;
            }

            var heroUserId = _sessionModule.HeroUserId;
            var friends = _friendsModule.Friends;
            var incomingInvites = friends.Where(friend => friend.State == FriendState.Pending && !friend.IsOutgoingInvite(heroUserId)).ToList();
            
            if (incomingInvites.IsExist(userId))
            {
                _friendsModule.AcceptFriend(userId, null);
                return;
            }

            if (_friendsModule.IsFriend(userId, true))
            {
                Bro.Log.Error("user already a friend");
                return;
            }
            
            if (userId == heroUserId)
            {
                Bro.Log.Error("can not invite yourself");
                return;
            } 
            
            if (!_friendsModule.CanInviteFriend())
            {
                Bro.Log.Error("limit is reached");
                return;
            }
            
            _friendsModule.AddFriend(userId, (result) =>
            {
                if (!result)
                {
                    Log.Error("the invitation failed");   
                }
                else
                {
                    OnButtonTabInvites();
                }
            });
        }

        private void OnButtonShareInvites()
        {  
            var heroUserId = _sessionModule.HeroUserId;
            var heroUserHash = IdentificatorHash.Encode(heroUserId);
            #warning todo to config
            var link = "###" + heroUserHash;
            _intentDataModule.InvokeShare(link);
        }

        private void OnButtonJoinCommunity()
        {
            
        }

        private void OnInputValueChanged(string value)
        {
            _buttonAddFriend.interactable = value.Length != 0;
        }
        
        private void UpdateContent()
        {
            var friends = _friendsModule.Friends;
            var heroUserId = _sessionModule.HeroUserId;
            var heroUserHash = IdentificatorHash.Encode(heroUserId);
            
            var actualFriends = friends.Where(friend => friend.State == FriendState.Friends).ToList();
            var incomingInvites = friends.Where(friend => friend.State == FriendState.Pending && !friend.IsOutgoingInvite(heroUserId)).ToList();
            var outgoingInvites = friends.Where(friend => friend.State == FriendState.Pending && friend.IsOutgoingInvite(heroUserId)).ToList();
            var isNoInvites = incomingInvites.Count == 0 && outgoingInvites.Count == 0;
            var countFriendsTotal = actualFriends.Count;
            var countFriendsOnline = actualFriends.Count(friend => friend.Status.IsOnline());
            
            _buttonTabFriends.gameObject.SetActive(_currentTab != Tab.Friends); 
            _buttonTabInvites.gameObject.SetActive(_currentTab != Tab.Invites);
            _objectTabFriendsSelected.SetActive(_currentTab == Tab.Friends);
            _objectTabInvitesSelected.SetActive(_currentTab == Tab.Invites);
            _objectTabFriends.SetActive(_currentTab == Tab.Friends);
            _objectTabInvites.SetActive(_currentTab == Tab.Invites);
            
            _objectNoInvites.SetActive(isNoInvites);
            
            _containerScrollOutgoingInvites.gameObject.SetActive(!isNoInvites);
            _containerScrollIncomingInvites.gameObject.SetActive(!isNoInvites);
            _objectCounterIncomingInvites.gameObject.SetActive(incomingInvites.Count > 0);

            _labelCountIncomingInvites.text = incomingInvites.Count.ToString();
            _labelCountFriendsAll.text = countFriendsTotal.ToString();
            _labelCountFriendsOnline.text = countFriendsOnline.ToString();
            _labelUserId.text = heroUserHash;

            UpdateFriendsTab(actualFriends);
            UpdateInvitesTab(outgoingInvites, incomingInvites);
        }

        private void UpdateFriendsTab(List<Friend> friends)
        {
            RemoveNotPresentedItems(friends, _itemsFriends);
            UpdatePresentedFriends(friends, _itemsFriends);
            CreateNotPresentedFriends(friends, _itemsFriends, _prefabFriend, _containerScrollFriends);
        }
        
        private void UpdateInvitesTab(List<Friend> outgoingInvites, List<Friend> incomingInvites)
        {
            RemoveNotPresentedItems(outgoingInvites, _itemsOutgoingInvites);
            RemoveNotPresentedItems(incomingInvites, _itemsIncomingInvites);
            
            UpdatePresentedFriends(outgoingInvites, _itemsOutgoingInvites);
            UpdatePresentedFriends(incomingInvites, _itemsIncomingInvites);
            
            CreateNotPresentedFriends(outgoingInvites, _itemsOutgoingInvites, _prefabInvite, _containerScrollOutgoingInvites);
            CreateNotPresentedFriends(incomingInvites, _itemsIncomingInvites, _prefabInvite, _containerScrollIncomingInvites);
        }

        private void RemoveNotPresentedItems(List<Friend> friends, List<FriendsWindowItem> items)
        {
            for (var i = items.Count - 1; i >= 0; --i)
            {
                var item = items[i];
                if (!friends.IsExist(item.UserId))
                {
                    Destroy(item.gameObject);
                    items.RemoveAt(i);
                }
            }
        }

        private void UpdatePresentedFriends(List<Friend> friends, List<FriendsWindowItem> items)
        {
            foreach (var friend in friends)
            {
                var item = items.Get(friend.UserId);
                if (item != null)
                {
                    item.Setup(friend, Context, OnFriendPressed);
                }
            }
        }

        private void CreateNotPresentedFriends(List<Friend> friends, List<FriendsWindowItem> items, GameObject prefab, Transform root)
        {
            for (var i = friends.Count - 1; i >= 0; --i)
            {
                var friend = friends[i];
                if (!items.IsExist(friend.UserId))
                {
                    var itemObject = Instantiate(prefab, root);
                    var itemScript = itemObject.GetComponent<FriendsWindowItem>();
                    itemScript.Setup(friend, Context, OnFriendPressed);
                    items.Add(itemScript);
                }
            }
        }

        private void OnFriendPressed(FriendsWindowItemFriend friendItem)
        {
            _controlPopup.Show(friendItem, _friendsModule);
        }
    }
}