using System.Collections.Generic;

namespace Bro.Sketch.Client.Friends
{
    public static class ChatWindowExtension
    {
        public static bool IsExist(this List<ChatWindowGroupItem> conversations, long conversationId)
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
        
        public static ChatWindowGroupItem Get(this List<ChatWindowGroupItem> conversations, long conversationId)
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
    }
}