using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileSaveRequest : ServiceRequest<ProfileSaveRequest>
    {
        public readonly IntParam UserId = new IntParam();
        public readonly StringParam UserName = new StringParam();
        public readonly StringParam SaveData = new StringParam();
        
        public ProfileSaveRequest() : base(Network.Request.OperationCode.Infrastructure.ProfileSave, new ProfileChannel() )
        {
            AddParam(UserId);
            AddParam(UserName);
            AddParam(SaveData);
        }
    }
}