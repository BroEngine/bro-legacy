using System.Collections.Generic;

namespace Bro.Sketch.Client
{
    public static class FriendExtension
    {
        public static bool IsOutgoingInvite(this Friend friend, int heroUserId)
        {
            return friend.AuthorUserId == heroUserId;
        } 
        
        public static bool IsExist(this List<Friend> friends, int userId)
        {
            if (friends != null)
            {
                foreach (var friend in friends)
                {
                    if (friend.UserId == userId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}