namespace Bro.Sketch
{
    public class UserIdentity
    {
        public readonly PlatformType PlatformType;
        public readonly string DeviceId;
        
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        
        public string GoogleId { get; private set; }
        public string AppleId { get; private set; }
        public string FacebookId { get; private set; }

        public UserIdentity(int userId)
        {
            UserId = userId;
        }

        public UserIdentity( PlatformType platformType, string deviceId, string googleId, string appleId, string facebookId)
        {
            PlatformType = platformType;
            DeviceId = deviceId;
            GoogleId = googleId;
            AppleId = appleId;
            FacebookId = facebookId;
        }

        public bool IsBindingsExist
        {
            get
            {
                return ! string.IsNullOrEmpty(GoogleId) || ! string.IsNullOrEmpty(AppleId) || ! string.IsNullOrEmpty(FacebookId);
            }
        }

        public void UpdateBindings( string googleId, string appleId, string facebookId)
        {
            if (string.IsNullOrEmpty(GoogleId) && !string.IsNullOrEmpty(googleId))
            {
                GoogleId = googleId;
            }
            
            if (string.IsNullOrEmpty(AppleId) && !string.IsNullOrEmpty(appleId))
            {
                AppleId = appleId;
            }
            
            if (string.IsNullOrEmpty(FacebookId) && !string.IsNullOrEmpty(facebookId))
            {
                FacebookId = facebookId;
            }
        }
        
        public void UpdateAuthorization(int userId, string googleId, string appleId, string facebookId)
        {
            UserId = userId;
            UpdateBindings(googleId, appleId, facebookId);
        }

        public void SetUserName(string userName)
        {
            UserName = userName;
        }

        public override string ToString()
        {
            return "id = " + UserId + "; device = " + DeviceId + "; name = " + UserName + "; google = " + GoogleId + "; apple = " + AppleId + "; fb = " + FacebookId;
        }

        public UserIdentity Clone()
        {
            return new UserIdentity( PlatformType, DeviceId, GoogleId, AppleId, FacebookId) { UserId = UserId, UserName = UserName };
        }
    }
}