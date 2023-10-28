using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupGetResponse : ServiceResponse<GroupGetResponse>
    {
        public readonly GroupParam Group = new GroupParam();
        
        public GroupGetResponse() : base(Request.OperationCode.Social.GroupGet, new ProfileChannel() )
        {
            AddParam(Group);    
        }
    }
}