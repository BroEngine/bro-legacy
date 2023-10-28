using System.Collections.Generic;

namespace Bro.Sketch.Client.Friends
{
    public static class FriendsWindowExtension
    {
        public static bool IsExist(this List<FriendsWindowItem> friends, int userId)
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
        
        public static FriendsWindowItem Get(this List<FriendsWindowItem> friends, int userId)
        {
            if (friends != null)
            {
                foreach (var friend in friends)
                {
                    if (friend.UserId == userId)
                    {
                        return friend;
                    }
                }
            }

            return null;
        }
    }
}