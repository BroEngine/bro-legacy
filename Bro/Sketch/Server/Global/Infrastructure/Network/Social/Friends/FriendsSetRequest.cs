using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsSetRequest : ServiceRequest<FriendsSetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        public readonly IntParam AuthorUserId = new IntParam();
        public readonly ByteParam State = new ByteParam();
        
        public FriendsSetRequest() : base(Network.Request.OperationCode.Social.FriendsSet, new ProfileChannel() )
        {
            AddParam(UserId);
            AddParam(AuthorUserId);
            AddParam(State);
        }
    }
}