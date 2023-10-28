using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendIdentityParam : ParamsCollection
    {
        private readonly IntParam _userId = new IntParam();
        private readonly StringParam _facebookId = new StringParam();
        private readonly StringParam _googleId = new StringParam();
        private readonly StringParam _appleId = new StringParam();
       
        public FriendIdentityParam() : base(false)
        {
            AddParam(_userId);
            AddParam(_facebookId);
            AddParam(_googleId);
            AddParam(_appleId);
        }
        
        public FriendIdentity Value
        {
            get
            {
                if (!IsInitialized)
                {
                    Bro.Log.Error("param :: friend identity param are not initialized");
                }

                return new FriendIdentity()
                {
                    UserId = _userId.Value,
                    FacebookId = _facebookId.Value,
                    GoogleId = _googleId.Value,
                    AppleId = _appleId.Value
                };
            }
            set
            {
                _userId.Value = value.UserId;
                _facebookId.Value = value.FacebookId;
                _googleId.Value = value.GoogleId;
                _appleId.Value = value.AppleId;
                IsInitialized = true;
            }
        }
        
        public FriendIdentityParam Clone()
        {
            var param = NetworkPool.GetParam<FriendIdentityParam>();
            param.Value = Value;
            return param;
        }
    }
}