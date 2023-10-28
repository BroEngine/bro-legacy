using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileLoadResponse : ServiceResponse<ProfileLoadResponse>
    {
        public readonly IntParam UserId = new IntParam();
        public readonly StringParam GoogleId = new StringParam();
        public readonly StringParam AppleId = new StringParam();
        public readonly StringParam FacebookId = new StringParam();
        public readonly StringParam Name = new StringParam();
        public readonly StringParam Save = new StringParam();
        
        public ProfileLoadResponse() : base(Network.Request.OperationCode.Infrastructure.ProfileLoad, new ProfileChannel() )
        {
            AddParam(UserId);
            AddParam(GoogleId);
            AddParam(AppleId);
            AddParam(FacebookId);
            AddParam(Name);
            AddParam(Save);
        }
    }
}