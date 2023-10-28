using System.Collections.Generic;
using System.Linq;

namespace Bro.Sketch.Client
{
    public class ConversationStorage
    {
        private readonly Dictionary<int, List<Conversation>> _conversations = new Dictionary<int, List<Conversation>>();

        public List<Conversation> Conversations
        {
            get
            {
                var result = new List<Conversation>();
                foreach (var conversationPair in _conversations)
                {
                    var conversation = conversationPair.Value;
                    result = result.Concat(conversation).ToList();
                }
                return result;
            }
        }

        public void SetConversations(int userId, List<Conversation> conversations)
        {
            _conversations[userId] = conversations;
        }
    }
}