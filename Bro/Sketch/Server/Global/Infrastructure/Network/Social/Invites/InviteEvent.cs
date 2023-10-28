using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class InviteEvent : ServiceEvent<InviteEvent>
    {
        public readonly ByteParam Action = new ByteParam();
        public readonly IntParam TargetUserId = new IntParam();
        public readonly IntParam AuthorUserId = new IntParam();
        public readonly StringParam Data = new StringParam();
        
        public InviteEvent() : base(Event.OperationCode.Social.Invite, null )
        {
            AddParam(Action);
            AddParam(TargetUserId);
            AddParam(AuthorUserId);
            AddParam(Data);
        }
    }
}