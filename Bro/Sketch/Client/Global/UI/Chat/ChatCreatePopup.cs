using Bro.Toolbox.Client.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Friends
{
    public class ChatCreatePopupArgs : IWindowArgs
    {
        public enum CreateType
        {
            Direct,
            Public
        }

        public readonly CreateType Type;
        
        public ChatCreatePopupArgs(CreateType type)
        {
            Type = type;
        }
    }

    [Window(WindowItemType.Popup)]
    public class ChatCreatePopup : Window
    {
        [SerializeField] private Text _valuePlaceholder;
        [SerializeField] private InputField _inputValue;
        [SerializeField] private InputField _inputMeta;
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private Button _buttonCreate;

        private ChatCreatePopupArgs.CreateType _type;
        private SessionModule _sessionModule;
        private ConversationModule _conversationModule;
        
        private void Awake()
        {
            _buttonCancel.onClick.AddListener(OnButtonCancel);
            _buttonCreate.onClick.AddListener(OnButtonCreate);
        }

        protected override void OnShow(IWindowArgs args)
        {
            _sessionModule = Context.Application.GlobalContext.GetModule<SessionModule>();
            _conversationModule = Context.Application.GlobalContext.GetModule<ConversationModule>();
            
            var a = (ChatCreatePopupArgs) args;
            _type = a.Type;

            switch (_type)
            {
                case ChatCreatePopupArgs.CreateType.Direct:
                    _valuePlaceholder.text = "User Id";
                    break;
                case ChatCreatePopupArgs.CreateType.Public:
                    _valuePlaceholder.text = "Title";
                    break;
            }

            _inputMeta.text = string.Empty;
            _inputValue.text = string.Empty;
        }

        private void OnButtonCancel()
        {
            Context.GetModule<UIModule>().DirectlyHide<ChatCreatePopup>();
        }

        private void OnButtonCreate()
        {
            switch (_type)
            {
                case ChatCreatePopupArgs.CreateType.Direct:
                    var user01 = _sessionModule.HeroUserId;
                    var user02 = int.Parse( _inputValue.text);
                    var directId = Conversation.CreateDirectConversationId();

                    var privateData = new ConversationPrivate();
                    privateData.Users.Add(user01, "user 01 name");
                    privateData.Users.Add(user02, "user 02 name");
                    
                    
                    
                    
                    
                    _conversationModule.Join(directId, user01, privateData.Serialize(), _inputMeta.text);
                    _conversationModule.Join(directId, user02);
                    
                    break;
                case ChatCreatePopupArgs.CreateType.Public:
                    var userId = _sessionModule.HeroUserId;
                    var conversationId = Conversation.CreatePublicConversationId();
                    _conversationModule.Join(conversationId, userId, _inputValue.text, _inputMeta.text);
                    break;
            }
            
            Context.GetModule<UIModule>().DirectlyHide<ChatCreatePopup>();
        }
    }
}