using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileSaveResponse : ServiceResponse<ProfileSaveResponse>
    {
        public readonly BoolParam Successfully = new BoolParam();
        
        public ProfileSaveResponse() : base(Network.Request.OperationCode.Infrastructure.ProfileSave, new ProfileChannel())
        {
            AddParam(Successfully);
        }
    }
}