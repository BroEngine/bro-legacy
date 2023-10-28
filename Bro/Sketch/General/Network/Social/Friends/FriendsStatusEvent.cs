using Bro.Network;

namespace Bro.Sketch.Network
{
    public class FriendsStatusEvent : NetworkEvent<FriendsStatusEvent>
    {
        public readonly FriendParam Friend = new FriendParam();   
        
        public FriendsStatusEvent() : base(Event.OperationCode.Social.FriendsStatus)
        {
            AddParam(Friend);
        }
    }
}