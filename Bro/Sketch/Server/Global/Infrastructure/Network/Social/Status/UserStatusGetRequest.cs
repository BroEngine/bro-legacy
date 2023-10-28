using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class UserStatusGetRequest : ServiceRequest<UserStatusGetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public UserStatusGetRequest() : base(Network.Request.OperationCode.Social.UserStatusGet, new ProfileChannel() )
        {
            AddParam(UserId);   
        }
    }
}