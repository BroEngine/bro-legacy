using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsIdentityResponse : ServiceResponse<FriendsIdentityResponse>
    {
        public readonly ArrayParam<FriendIdentityParam> Friends = new ArrayParam<FriendIdentityParam>(short.MaxValue);

        public FriendsIdentityResponse() : base(Request.OperationCode.Social.FriendsIdentity, new ProfileChannel() )
        {
            AddParam(Friends);
        }
    }
}