using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsDeleteRequest : NetworkRequest<FriendsDeleteRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public FriendsDeleteRequest() : base(Request.OperationCode.Social.FriendsDelete)
        {
            AddParam(UserId);
        }
    }
}