using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsIdentityRequest : NetworkRequest<FriendsIdentityRequest>
    {
        public readonly ArrayParam<FriendIdentityParam> Friends = new ArrayParam<FriendIdentityParam>(short.MaxValue);
        
        public FriendsIdentityRequest() : base(Request.OperationCode.Social.FriendsIdentity)
        {
            AddParam(Friends);
        }
    }
}