namespace Bro.Sketch
{
    public class FriendIdentity /* at least one property must be set */
    {
        public int UserId;
        public string FacebookId;
        public string GoogleId;
        public string AppleId;

        public bool IsIdentified
        {
            get { return UserId > 0; }
        }
    }
}