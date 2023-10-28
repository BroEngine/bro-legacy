using System.Collections.Generic;

namespace Bro.Sketch
{
    public static class ConversationExtension
    {
        public static List<long> GetIds(this List<Conversation> conversations)
        {
            var ids = new List<long>();
            foreach (var conversation in conversations)
            {
                ids.Add(conversation.ConversationId);
            }
            return ids;
        }

        public static bool IsExist(this List<Conversation> conversations, long conversationId)
        {
            if (conversations != null)
            {
                foreach (var conversation in conversations)
                {
                    if (conversation.ConversationId == conversationId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }   
        
        public static Conversation Get(this List<Conversation> conversations, long conversationId)
        {
            if (conversations != null)
            {
                foreach (var conversation in conversations)
                {
                    if (conversation.ConversationId == conversationId)
                    {
                        return conversation;
                    }
                }
            }

            return null;
        }  
        
        public static bool IsExist(this List<ConversationMessage> messages, long timestamp)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    if (message.Timestamp == timestamp)
                    {
                        return true;
                    }
                }
            }

            return false;
        }    
        
        public static void Remove(this List<ConversationMessage> messages, long timestamp)
        {
            if (messages != null)
            {
                for (var i = messages.Count - 1; i >= 0; --i)
                {
                    var message = messages[i];
                    if (message.Timestamp == timestamp)
                    {
                        messages.RemoveAt(i);
                    }
                }
            }
        }       
    }
}