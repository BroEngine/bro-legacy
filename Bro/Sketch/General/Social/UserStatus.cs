using Bro.Json;

namespace Bro.Sketch
{
    public class UserStatus
    {
        [JsonProperty("user_id")] public int UserId;
        [JsonProperty("session_id")] public int SessionId;
        [JsonProperty("state")] public byte State;
        [JsonProperty("timestamp")] public long Timestamp;
        [JsonProperty("channel")] public string Channel;
        [JsonProperty("name")] public string Name;
        [JsonProperty("data")] public string Data;
        
        public static UserStatus Default(int userId)
        {
            return new UserStatus()
            {
                UserId = userId
            };
        }
    }
    
    public static class UserStatusExtension
    {
        public static bool IsOnline(this UserStatus status)
        {
            if (status == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(status.Channel))
            {
                return false;
            }

            var deltaTime = (TimeInfo.GlobalTimestamp - status.Timestamp);

            if (deltaTime > 15 * 60000L) // 15 min, todo to config
            {
                return false;
            }

            return status.State != 0;
        }
    }
}