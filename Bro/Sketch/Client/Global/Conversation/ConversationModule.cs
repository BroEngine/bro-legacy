using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class ConversationModule : IClientContextModule
    {
        private IClientContext _context;
        private NetworkEngine _engine;
        private ConversationStorage _storage;
        private ConversationMessagesStorage _messages;
        
        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();
        public List<Conversation> Conversations => _storage.Conversations;
        public ConversationMessagesStorage Messages => _messages;
        
        public virtual void Initialize(IClientContext context)
        {
            _storage = new ConversationStorage();
            _messages = new ConversationMessagesStorage(100);
            _context = context;
            _engine = _context.GetNetworkEngine();
        }

        public IEnumerator Load()
        {
            _context.AddDisposable(new NetworkEventObserver<Network.ConversationEvent>(OnConversationEvent, _engine));
            _context.AddDisposable(new NetworkEventObserver<Network.ConversationMessagesEvent>(OnMessagesEvent, _engine));
            yield return null;
        }

        public IEnumerator Unload()
        {
            yield return null;
        }

        private void OnConversationEvent(Network.ConversationEvent conversationEvent)
        {
            var userId = conversationEvent.UserId.Value;
            var conversations = conversationEvent.Conversations;
            _storage.SetConversations(userId, conversations);
            new ConversationEvent().Launch();
        }

        private void OnMessagesEvent(Network.ConversationMessagesEvent messagesEvent)
        {
            var conversationId = messagesEvent.ConversationId.Value;
            var messages = messagesEvent.Messages;
            _messages.Add(conversationId, messages);
            new ConversationMessageEvent(conversationId, messages).Launch();
        }

        public void Join(long conversationId, int userId, string title = "", string meta = "")
        {
            new ConversationActionTask(_context, conversationId, userId, ConversationAction.Join, title, meta).Launch(_context);
        }

        public void Leave(long conversationId, int userId)
        {
            new ConversationActionTask(_context, conversationId, userId, ConversationAction.Leave).Launch(_context);
        }

        public void Send(long conversationId, ConversationMessage message)
        {
            new ConversationMessageTask(_context, conversationId, message).Launch(_context);
        }
    }
}