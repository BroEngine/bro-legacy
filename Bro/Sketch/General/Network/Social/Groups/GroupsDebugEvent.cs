using Bro.Network;

namespace Bro.Sketch.Network
{
    public class GroupsDebugEvent : NetworkEvent<GroupsDebugEvent>
    {
        public readonly GroupParam Group = new GroupParam();   
        
        public GroupsDebugEvent() : base(Event.OperationCode.Social.GroupDebug)
        {
            AddParam(Group);
        }
    }
}