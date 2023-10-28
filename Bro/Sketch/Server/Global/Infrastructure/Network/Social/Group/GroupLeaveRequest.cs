using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupLeaveRequest : ServiceRequest<GroupLeaveRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public GroupLeaveRequest() : base(Network.Request.OperationCode.Social.GroupLeave, new ProfileChannel() )
        {
            AddParam(UserId);
        }
    }
}