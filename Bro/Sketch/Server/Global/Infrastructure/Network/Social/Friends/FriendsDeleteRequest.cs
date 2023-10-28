using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsDeleteRequest : ServiceRequest<FriendsDeleteRequest>
    {
        public readonly IntParam UserId01 = new IntParam();
        public readonly IntParam UserId02 = new IntParam();
        
        public FriendsDeleteRequest() : base(Network.Request.OperationCode.Social.FriendsDelete, new ProfileChannel() )
        {
            AddParam(UserId01);
            AddParam(UserId02);
        }
    }
}