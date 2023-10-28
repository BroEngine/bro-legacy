using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupLeaveResponse : ServiceResponse<GroupLeaveResponse>
    {
        public GroupLeaveResponse() : base(Request.OperationCode.Social.GroupLeave, new ProfileChannel() )
        {
            
        }
    }
}