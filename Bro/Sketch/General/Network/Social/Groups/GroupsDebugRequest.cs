using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class GroupsDebugRequest : NetworkRequest<GroupsDebugRequest>
    {
        public readonly ByteParam Command = new ByteParam();
        public readonly IntParam Argument = new IntParam();
        public readonly StringParam Data = new StringParam();
        
        public GroupsDebugRequest() : base(Request.OperationCode.Social.GroupDebug)
        {
            AddParam(Command);
            AddParam(Argument);
            AddParam(Data);
        }
    }
}