using System.Collections.Generic;
using System.Linq;
using Bro.Json;

namespace Bro.Sketch
{
    public class ConversationPrivate
    {
        [JsonProperty("users")] public Dictionary<int,string> Users = new Dictionary<int, string>();
        [JsonProperty("meta")] public Dictionary<int,string> Meta = new Dictionary<int, string>();
        
        public static ConversationPrivate Deserialize(string json)
        {
            if (json == null || string.IsNullOrEmpty(json))
            {
                return  new ConversationPrivate();
            }

            return JsonConvert.DeserializeObject<ConversationPrivate>(json, JsonSettings.AutoSettings);
        }

        public string GetName(int userId)
        {
            if (Users.ContainsKey(userId))
            {
                return Users[userId];
            }
            return string.Empty;
        }

        public string GetMeta(int userId)
        {
            if (Meta.ContainsKey(userId))
            {
                return Meta[userId];
            }
            return string.Empty;
        }

        public int GetAnotherUserId(int heroUserId)
        {
            foreach (var userPair in Users)
            {
                if (userPair.Key != heroUserId)
                {
                    return userPair.Key;
                }
            }
            return 0;
        }

        public string Serialize()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.None, JsonSettings.AutoSettings);
            return data;
        }
    }
}