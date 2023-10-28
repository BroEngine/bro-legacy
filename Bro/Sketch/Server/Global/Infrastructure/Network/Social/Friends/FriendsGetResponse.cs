using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsGetResponse : ServiceResponse<FriendsGetResponse>
    {
        public readonly ArrayParam<FriendParam> Friends = new ArrayParam<FriendParam>(short.MaxValue);

        public FriendsGetResponse() : base(Request.OperationCode.Social.FriendsGet, new ProfileChannel() )
        {
            AddParam(Friends);
        }
    }
}