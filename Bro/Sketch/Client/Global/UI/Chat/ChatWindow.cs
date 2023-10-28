using System;
using System.Collections.Generic;
using Bro.Client;
using Bro.Toolbox.Client.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Friends
{
    [Window()]
    public class ChatWindow : Window
    {
        private int _limit = 20;
        
        [SerializeField] private GameObject _prefabGroup;
        [SerializeField] private Transform _containerGroup;
        
        [SerializeField] private Button _buttonClose;
        [SerializeField] private Button _buttonSend;
        [SerializeField] private Button _buttonLeave;
        [SerializeField] private Button _buttonAdd;
        [SerializeField] private Button _buttonCreateChat;
        [SerializeField] private Button _buttonCreatePrivate;
        
        [SerializeField] private InputField _inputMessage;
        [SerializeField] private InputField _inputUserId;

        [SerializeField] private Text _textChat;

        private long _selectedConversationId;
        private SessionModule _sessionModule;
        private ConversationModule _conversationModule;
        
        private readonly EventObserver<ConversationEvent> _conversationEventObserver = new EventObserver<ConversationEvent>();
        private readonly EventObserver<ConversationMessageEvent> _messagesEventObserver = new EventObserver<ConversationMessageEvent>();
        private readonly List<ChatWindowGroupItem> _conversations = new List<ChatWindowGroupItem>();
        
        private void Awake()
        {
            _buttonClose.onClick.AddListener(OnButtonClose);
            _buttonSend.onClick.AddListener(OnButtonSend);
            
            _buttonLeave.onClick.AddListener(OnButtonLeave);
            _buttonAdd.onClick.AddListener(OnButtonAdd);
            _buttonCreateChat.onClick.AddListener(OnButtonCreateChat);
            _buttonCreatePrivate.onClick.AddListener(OnButtonCreatePrivate);
        }

        private void OnEnable()
        {
            _conversationEventObserver.Subscribe(OnConversationEvent);
            _messagesEventObserver.Subscribe(OnMessagesEvent);
        }

        private void OnDisable()
        {
            _conversationEventObserver.Unsubscribe();
            _messagesEventObserver.Unsubscribe();
        }

        protected override void OnShow(IWindowArgs args)
        {
            _sessionModule = Context.Application.GlobalContext.GetModule<SessionModule>();
            _conversationModule = Context.Application.GlobalContext.GetModule<ConversationModule>();
            
            ReDraw();
        }

        private void OnButtonSend()
        {
            var text = _inputMessage.text;
            if (!string.IsNullOrEmpty(text))
            {
                var message = new ConversationMessage();
                message.Text = text;
                message.UserId = _sessionModule.HeroUserId;
                message.UserName = "user_name_" + _sessionModule.HeroUserId;
                message.Timestamp = TimeInfo.GlobalTimestamp;
                message.Meta = "meta";
                
                _conversationModule.Send(_selectedConversationId, message);
            }

            _inputMessage.text = string.Empty;
        }

        private void OnButtonClose()
        {
            Context.GetModule<UIModule>().Hide();   
        }

        private void OnButtonLeave()
        {
            var userId = _sessionModule.HeroUserId;
            _conversationModule.Leave(_selectedConversationId, userId);
        }

        private void OnButtonAdd()
        {
            var userId = int.Parse(_inputUserId.text);
            _conversationModule.Join(_selectedConversationId, userId);

            _inputUserId.text = string.Empty;
        }

        private void OnButtonCreatePrivate()
        {
            Context.GetModule<UIModule>().Show<ChatCreatePopup>(new ChatCreatePopupArgs(ChatCreatePopupArgs.CreateType.Direct));
        }

        private void OnButtonCreateChat()
        {
            Context.GetModule<UIModule>().Show<ChatCreatePopup>(new ChatCreatePopupArgs(ChatCreatePopupArgs.CreateType.Public));
        }

        private void OnConversationEvent(ConversationEvent e)
        {
            ReDraw();
        }

        private void OnMessagesEvent(ConversationMessageEvent e)
        {
            ReDrawMessages();
        }

        private void ReDraw()
        {
            ReDrawConversations();
            ReDrawSelected();
            ReDrawMessages();
            ReDrawControls();
        }

        private void ReDrawConversations()
        {
            var conversations = _conversationModule.Conversations;

            RemoveNotPresentedItems(conversations, _conversations);
            UpdatePresentedFriends(conversations, _conversations);
            CreateNotPresentedFriends(conversations, _conversations, _prefabGroup, _containerGroup);
        }

        private void ReDrawSelected()
        {
            foreach (var conversation in _conversations)
            {
                conversation.SetSelected(conversation.ConversationId == _selectedConversationId);
            }
        }

        private void ReDrawMessages()
        {
            var messages = _conversationModule.Messages.Get(_selectedConversationId);
            
            if (messages.Count > _limit)
            {
                messages.RemoveRange(0, messages.Count - _limit);
            }
            
            var data = string.Empty;

            foreach (var message in messages)
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddMilliseconds( message.Timestamp  ).ToLocalTime();

                var time = "[" + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second + "]";
                var userName = "[" + message.UserId + ":" + message.UserName + "]";
                var item = time + " " + userName + " " + message.Text;
                data += item + "\n";
            }
            
            _textChat.text = data;
        }

        private void ReDrawControls()
        {
            var selectedExist = _conversations.IsExist(_selectedConversationId);

            var isGlobal = Conversation.IsGlobal(_selectedConversationId);
            var isPublic = Conversation.IsPublic(_selectedConversationId);
            var isDirect = Conversation.IsDirect(_selectedConversationId);
            var isSystem = Conversation.IsGroup(_selectedConversationId);
            
            _buttonAdd.gameObject.SetActive(isPublic);
            // _buttonLeave.gameObject.SetActive(isPublic || isDirect);
            _buttonSend.gameObject.SetActive(selectedExist);
            _inputUserId.gameObject.SetActive(isPublic);
            _inputMessage.gameObject.SetActive(selectedExist);
        }

        private void RemoveNotPresentedItems(List<Conversation> conversations, List<ChatWindowGroupItem> items)
        {
            for (var i = items.Count - 1; i >= 0; --i)
            {
                var item = items[i];
                if (!conversations.IsExist(item.ConversationId))
                {
                    Destroy(item.gameObject);
                    items.RemoveAt(i);
                }
            }
        }

        private void UpdatePresentedFriends(List<Conversation> conversations, List<ChatWindowGroupItem> items)
        {
            foreach (var conversation in conversations)
            {
                var item = items.Get(conversation.ConversationId);
                if (item != null)
                {
                    item.Setup(conversation, Context, OnConversationPressed);
                }
            }
        }

        private void CreateNotPresentedFriends(List<Conversation> conversations, List<ChatWindowGroupItem> items, GameObject prefab, Transform root)
        {
            for (var i = conversations.Count - 1; i >= 0; --i)
            {
                var conversation = conversations[i];
                if (!items.IsExist(conversation.ConversationId))
                {
                    var itemObject = Instantiate(prefab, root);
                    var itemScript = itemObject.GetComponent<ChatWindowGroupItem>();
                    itemScript.Setup(conversation, Context, OnConversationPressed);
                    items.Add(itemScript);
                }
            }
        }

        private void OnConversationPressed(long conversationId)
        {
            _selectedConversationId = conversationId;
            ReDrawSelected();
            ReDrawMessages();
            ReDrawControls();
        }
    }
}