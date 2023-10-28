using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsIdentityResponse : NetworkResponse<FriendsIdentityResponse>
    {
        public readonly ArrayParam<FriendIdentityParam> Friends = new ArrayParam<FriendIdentityParam>(short.MaxValue);

        public FriendsIdentityResponse() : base(Request.OperationCode.Social.FriendsIdentity)
        {
            AddParam(Friends);
        }
    }
}


