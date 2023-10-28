using System.Collections.Generic;

namespace Bro.Sketch
{
    public class Conversation
    {
        public long ConversationId;
        public string Title;
        public string Meta;
        public List<int> Users = new List<int>();
        
        public override string ToString()
        {
            var data = "{" + ConversationId + " " + Title + ":" + Meta + "}";
            return data;
        }

        public bool IsExist(int userId)
        {
            return Users.Contains(userId);
        }

        // system

        public const long Mask = 1000000000000000000;
        
        public const long MaskGlobal = 1000000000000000000;
        public const long MaskDirect = 7000000000000000000;
        public const long MaskPublic = 5000000000000000000;
        public const long MaskGroup = 3000000000000000000;

        public static long CreateGlobalConversationId(long id)
        {
            return MaskGlobal + id;
        }
        
        public static long CreateDirectConversationId()
        {
            return MaskDirect + TimeInfo.GlobalTimestamp;
        }   

        public static long CreatePublicConversationId()
        {
            return MaskPublic + TimeInfo.GlobalTimestamp;
        }  
        
        public static long CreateGroupConversationId(long id)
        {
            return MaskGroup + id;
        }
        
        public static bool IsGlobal(long conversationId)
        {
            return conversationId - (conversationId % Mask) == MaskGlobal;
        } 
        
        public static bool IsDirect(long conversationId)
        {
            return conversationId - (conversationId % Mask) == MaskDirect;
        }
        
        public static bool IsPublic(long conversationId)
        {
            return conversationId - (conversationId % Mask) == MaskPublic;
        }

        public static bool IsGroup(long conversationId)
        {
            return conversationId - (conversationId % Mask) == MaskGroup;
        }
    }
}