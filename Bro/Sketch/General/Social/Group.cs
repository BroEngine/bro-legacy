using System.Collections.Generic;
using Bro.Json;

namespace Bro.Sketch
{
    #pragma warning disable 660,661
    public class Group
    {
        [JsonProperty("group_id")] public long GroupId;
        [JsonProperty("users")] public List<int> Users = new List<int>();
        [JsonProperty("data")] public string Data;
        [JsonProperty("timestamp")] public long Timestamp;
        
        public override string ToString()
        {
            var users = string.Join(",", Users); 
            return "group id = " + GroupId + "; size = " + Users.Count + "; users = [" + users + "]";
        }
    }
}