using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsGetRequest : ServiceRequest<FriendsGetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public FriendsGetRequest() : base(Network.Request.OperationCode.Social.FriendsGet, new ProfileChannel() )
        {
            AddParam(UserId);
        }
    }
}