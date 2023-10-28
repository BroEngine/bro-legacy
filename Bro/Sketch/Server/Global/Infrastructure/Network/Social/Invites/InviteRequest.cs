using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class InviteRequest : ServiceRequest<InviteRequest>
    {
        public readonly ByteParam Action = new ByteParam();
        public readonly IntParam TargetUserId = new IntParam();
        public readonly IntParam AuthorUserId = new IntParam();
        public readonly StringParam Data = new StringParam();
        
        public InviteRequest() : base(Network.Request.OperationCode.Social.Invite, new ProfileChannel() )
        {
            AddParam(Action);
            AddParam(TargetUserId);
            AddParam(AuthorUserId);
            AddParam(Data);
        }
    }
}