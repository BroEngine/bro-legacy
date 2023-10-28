using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsStatusEvent : ServiceEvent<FriendsStatusEvent>
    {
        public readonly IntParam DestinationUserId = new IntParam();
        public readonly FriendParam Friend = new FriendParam();
        
        public FriendsStatusEvent() : base(Event.OperationCode.Social.FriendsStatus, null )
        {
            AddParam(DestinationUserId);
            AddParam(Friend);
        }
    }
}