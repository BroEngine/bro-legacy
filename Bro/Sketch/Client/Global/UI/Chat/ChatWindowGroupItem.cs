using System;
using Bro.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Friends
{
    public class ChatWindowGroupItem : MonoBehaviour
    {
        [SerializeField] private Text _textId;
        [SerializeField] private Text _textTitle;
        [SerializeField] private Text _textMeta;
        
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selected;

        public long ConversationId => _conversation.ConversationId;

        private Conversation _conversation;
        private IClientContext _context;
        private Action<long> _callback;

        private void Awake()
        {
            _button.onClick.AddListener(OnButton);
        }

        public void Setup(Conversation conversation, IClientContext context, Action<long> callback)
        {
            _conversation = conversation;
            _context = context;
            _callback = callback;

            var conversationId = conversation.ConversationId;
            var isGlobal = Conversation.IsGlobal(conversationId);
            var isPublic = Conversation.IsPublic(conversationId);
            var isDirect = Conversation.IsDirect(conversationId);
            var isSystem = Conversation.IsGroup(conversationId);

            var title = "UNDEF";

            if (isGlobal)
            {
                title = "GLOBAL";
            }
            
            if (isPublic)
            {
                title = "PUBLIC";
            } 
            
            if (isDirect)
            {
                title = "DIRECT";
            }
            
            if (isSystem)
            {
                title = "SYSTEM";
            }

            title += " " + conversation.ConversationId;
            _textId.text = title;
            _textMeta.text = "size = " + _conversation.Users.Count + "; meta = " + _conversation.Meta;
            _textTitle.text = _conversation.Title;
        }

        public void SetSelected(bool selected)
        {
            _selected.SetActive(selected);
        }

        private void OnButton()
        {
            _callback?.Invoke(ConversationId);
        }
    }
}