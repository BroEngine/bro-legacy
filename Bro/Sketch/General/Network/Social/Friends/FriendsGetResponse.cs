using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsGetResponse  : NetworkResponse<FriendsGetResponse>
    {
        public readonly BoolParam IsServiceAvailable = new BoolParam();
        public readonly ArrayParam<FriendParam> Friends = new ArrayParam<FriendParam>(short.MaxValue);

        public FriendsGetResponse() : base(Request.OperationCode.Social.FriendsGet)
        {
            AddParam(IsServiceAvailable);
            AddParam(Friends);
        }
    }
}

