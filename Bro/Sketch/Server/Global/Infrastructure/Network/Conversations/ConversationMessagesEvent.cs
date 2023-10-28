using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationMessagesEvent : ServiceEvent<ConversationMessagesEvent>
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
        }
        
        public ConversationMessagesEvent() : base(Event.OperationCode.Social.Messages, null )
        {
            AddParam(ConversationId);
            AddParam(_messages);
        }
    }
}