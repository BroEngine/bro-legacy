using Bro.Network.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class InviteResponse : ServiceResponse<InviteResponse>
    {
        public InviteResponse() : base(Network.Request.OperationCode.Social.Invite, new ProfileChannel() )
        {
            
        }
    }
}