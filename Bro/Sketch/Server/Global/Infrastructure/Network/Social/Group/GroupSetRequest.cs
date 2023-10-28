using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupSetRequest : ServiceRequest<GroupSetRequest>
    {
        public readonly GroupParam Group = new GroupParam();
        
        public GroupSetRequest() : base(Network.Request.OperationCode.Social.GroupSet, new ProfileChannel() )
        {
            AddParam(Group);
        }
        
        public GroupSetRequest(Group g) : base(Network.Request.OperationCode.Social.GroupSet, new ProfileChannel() )
        {
            AddParam(Group);
            Group.Value = g;
        }
    }
}