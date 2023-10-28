using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupGetRequest : ServiceRequest<GroupGetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public GroupGetRequest() : base(Network.Request.OperationCode.Social.GroupGet, new ProfileChannel() )
        {
            AddParam(UserId);
        }
    }
}