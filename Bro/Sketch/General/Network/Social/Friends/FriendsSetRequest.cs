using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsSetRequest : NetworkRequest<FriendsSetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        public readonly ByteParam State = new ByteParam();
        
        public FriendsSetRequest() : base(Request.OperationCode.Social.FriendsSet)
        {
            AddParam(UserId);
            AddParam(State);
        }
    }
}