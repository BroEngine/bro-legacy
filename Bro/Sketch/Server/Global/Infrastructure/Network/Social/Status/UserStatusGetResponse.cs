using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class UserStatusGetResponse : ServiceResponse<UserStatusGetResponse>
    {
        public readonly UserStatusParam Status = new UserStatusParam();
        
        public UserStatusGetResponse() : base(Request.OperationCode.Social.UserStatusGet, new ProfileChannel() )
        {
            AddParam(Status);
        }
    }
}