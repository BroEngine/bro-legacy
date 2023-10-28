namespace Bro.Sketch.Server
{
    public class Profile
    {
        private readonly Actor _actor;
        public short OwnerId { get; private set; }
        public int UserId => UserIdentity.UserId;
        public UserIdentity UserIdentity { get; private set; }
        public object Data { get; private set; }
        
        public Profile(Actor actor)
        {
            _actor = actor;
        }

        public void SetData(UserIdentity userIdentity, object data)
        {
            UserIdentity = userIdentity;
            Data = data;
        }

        public void SetOwnerId(short ownerId)
        {
            OwnerId = ownerId;
        }
    }
}