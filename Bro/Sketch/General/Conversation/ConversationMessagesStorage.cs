using System.Collections.Generic;
using System.Linq;

namespace Bro.Sketch
{
    public class ConversationMessagesStorage
    {
        private readonly int _limit;
        private readonly Dictionary<long, List<ConversationMessage>> _data = new Dictionary<long, List<ConversationMessage>>();
        
        public ConversationMessagesStorage(int limit)
        {
            _limit = limit;
        }

        public void Add(long conversationId, List<ConversationMessage> messages)
        {
            if (!_data.ContainsKey(conversationId))
            {
                _data[conversationId] = new List<ConversationMessage>();
            }

            foreach (var message in messages)
            {
                _data[conversationId].Add(message);
            }
            
            // remove duplicates and sort
            var enumerable = _data[conversationId].GroupBy(x => x.Timestamp).Select(y => y.First());
            enumerable = enumerable.OrderBy(order => order.Timestamp);
            
            // limit size
            _data[conversationId] = enumerable.ToList();
            if (_data[conversationId].Count > _limit)
            {
                _data[conversationId].RemoveRange(0, _data[conversationId].Count - _limit);
            }
        }

        public List<ConversationMessage> Get(long conversationId, int size = 0)
        {
            if (!_data.ContainsKey(conversationId))
            {
                return new List<ConversationMessage>();
            }
            var data = _data[conversationId].ToList();

            if (size > 0 && data.Count > size)
            {
                data.RemoveRange(0, _data[conversationId].Count - size);
            }

            return data;
        }
    }
}