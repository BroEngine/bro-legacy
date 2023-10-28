using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class GroupStatusEvent : ServiceEvent<GroupStatusEvent>
    {
        public readonly IntParam DestinationUserId = new IntParam();
        public readonly GroupParam Group = new GroupParam();
        
        public GroupStatusEvent() : base(Event.OperationCode.Social.GroupStatus, null )
        {
            AddParam(DestinationUserId);
            AddParam(Group);
        }
    }
}