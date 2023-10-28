using Bro.Json;

namespace Bro.Sketch
{
    #pragma warning disable 660,661
    public class Friend
    {
        [JsonProperty("user_id")] public int UserId;
        [JsonProperty("author_user_id")] public int AuthorUserId;
        [JsonProperty("status")] public UserStatus Status;
        [JsonProperty("state")] public FriendState State;
        [JsonProperty("timestamp")] public long Timestamp;
        
        public static bool operator == (Friend a, Friend b)
        {
            var aNull = ReferenceEquals(a, null);
            var bNull = ReferenceEquals(b, null);
            
            return (!aNull || bNull) && (aNull || !bNull) && (aNull || a.UserId == b.UserId);
        }

        public static bool operator !=(Friend p1, Friend p2)
        {
            var p1Null = ReferenceEquals(p1, null);
            var p2Null = ReferenceEquals(p2, null);
            return (p1Null && !p2Null) || (!p1Null && p2Null) || (!p1Null && p1.UserId != p2.UserId);
        }

        public override string ToString()
        {
            return "user id: " + UserId + "; author user id = " + AuthorUserId + "; state = " + State.GetDescription();
        }
    }
}