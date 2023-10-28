using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsIdentityRequest : ServiceRequest<FriendsIdentityRequest>
    {
        public readonly ArrayParam<FriendIdentityParam> Friends = new ArrayParam<FriendIdentityParam>(short.MaxValue);

        public FriendsIdentityRequest() : base(Network.Request.OperationCode.Social.FriendsIdentity, new ProfileChannel() )
        {
            AddParam(Friends);
        }
    }
}
