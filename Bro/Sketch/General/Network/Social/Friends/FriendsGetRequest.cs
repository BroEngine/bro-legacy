using Bro.Network;

namespace Bro.Sketch.Network
{
    public class FriendsGetRequest : NetworkRequest<FriendsGetRequest>
    {
        public FriendsGetRequest() : base(Request.OperationCode.Social.FriendsGet)
        {

        }
    }
}