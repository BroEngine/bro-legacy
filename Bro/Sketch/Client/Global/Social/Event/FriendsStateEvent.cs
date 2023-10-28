using Bro.Client;

namespace Bro.Sketch.Client
{
    public class FriendsStateEvent : Event
    {
        public readonly FriendState State;
        public readonly Friend Friend;
        
        public FriendsStateEvent(FriendState state, Friend friend)
        {
            State = state;
            Friend = friend;
        }
    }
}