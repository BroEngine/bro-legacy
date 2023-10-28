using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationEvent : ServiceEvent<ConversationEvent>
    {
        private readonly ArrayParam<ConversationParam> _conversations = new ArrayParam<ConversationParam>(short.MaxValue);

        public readonly IntParam UserId = new IntParam();

        public List<Conversation> Conversations
        {
            get
            {
                var list = new List<Conversation>();
                foreach (var conversationParam in _conversations.Params)
                {
                    var conversation = conversationParam.Value;
                    list.Add(conversation);
                }
                return list;
            }
        }
        
        public ConversationEvent() : base(Event.OperationCode.Social.Conversations, null )
        {
            AddParam(UserId);
            AddParam(_conversations);
        }
    }
}