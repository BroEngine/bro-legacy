using Bro.Network.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class UserStatusSetRequest : ServiceRequest<UserStatusSetRequest>
    {
        public readonly UserStatusParam Status = new UserStatusParam();
        
        public UserStatusSetRequest() : base(Network.Request.OperationCode.Social.UserStatusSet, new ProfileChannel() )
        {
            AddParam(Status);
        }
    }
}