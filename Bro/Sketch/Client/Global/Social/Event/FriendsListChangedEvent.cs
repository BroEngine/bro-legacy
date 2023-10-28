using System.Collections.Generic;
using Bro.Client;

namespace Bro.Sketch.Client
{
    public class FriendsListChangedEvent : Event
    {
        public readonly List<Friend> Friends;

        public FriendsListChangedEvent(List<Friend> friends)
        {
            Friends = friends;
        }
    }
}