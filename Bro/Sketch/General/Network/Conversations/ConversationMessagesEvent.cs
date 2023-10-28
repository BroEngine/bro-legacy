using System.Collections.Generic;
using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationMessagesEvent : NetworkEvent<ConversationMessagesEvent>
    {
        public readonly LongParam ConversationId = new LongParam();
        private readonly ArrayParam<ConversationMessageParam> _messages = new ArrayParam<ConversationMessageParam>(byte.MaxValue);
        
        public List<ConversationMessage> Messages
        {
            get
            {
                var list = new List<ConversationMessage>();
                foreach (var messageParam in _messages.Params)
                {
                    var message = messageParam.Value;
                    list.Add(message);
                }
                return list;
            }
            
            set
            {
                _messages.Params.Clear();

                foreach (var item in value)
                {
                    var conversationParam = NetworkPool.GetParam<ConversationMessageParam>();
                    conversationParam.Value = item;
                    _messages.Add(conversationParam);
                }
            }
        }
        
        public ConversationMessagesEvent() : base(Event.OperationCode.Social.Messages)
        {
            AddParam(ConversationId);
            AddParam(_messages);
        }       
    }
}