using System.Collections.Generic;
using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationEvent : NetworkEvent<ConversationEvent>
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
            set
            {
                _conversations.Params.Clear();

                foreach (var item in value)
                {
                    var conversationParam = NetworkPool.GetParam<ConversationParam>();
                    conversationParam.Value = item;
                    _conversations.Add(conversationParam);
                }
            }
        }
        
        public ConversationEvent() : base(Event.OperationCode.Social.Conversations)
        {
            AddParam(UserId);
            AddParam(_conversations);
        }
    }
}