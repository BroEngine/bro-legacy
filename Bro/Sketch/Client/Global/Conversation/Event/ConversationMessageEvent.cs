using System.Collections.Generic;
using Bro.Client;

namespace Bro.Sketch.Client
{
    public class ConversationMessageEvent : Event
    {
        public readonly long ConversationId;
        public readonly List<ConversationMessage> Messages;
        
        public ConversationMessageEvent(long conversationId, List<ConversationMessage> messages)
        {
            ConversationId = conversationId;
            Messages = messages;
        }
    }
}