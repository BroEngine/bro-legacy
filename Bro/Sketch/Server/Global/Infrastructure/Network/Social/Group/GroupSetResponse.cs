using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupSetResponse : ServiceResponse<GroupSetResponse>
    {
        public readonly GroupParam Group = new GroupParam();
                
        public GroupSetResponse() : base(Request.OperationCode.Social.GroupSet, new ProfileChannel() )
        {
            AddParam(Group);
        }
    }
}

