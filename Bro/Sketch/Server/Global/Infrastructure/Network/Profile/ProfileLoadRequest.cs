using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileLoadRequest : ServiceRequest<ProfileLoadRequest>
    {
        public override int ExpirationTimestamp 
        {
            get { return 20000; }
        }
        
        public readonly IntParam UserId = new IntParam();
        public readonly StringParam DeviceId = new StringParam();
        public readonly StringParam GoogleId = new StringParam();
        public readonly StringParam AppleId = new StringParam();
        public readonly StringParam FacebookId = new StringParam();
        public readonly BoolParam AutoCreate = new BoolParam();

        public ProfileLoadRequest() : base(Network.Request.OperationCode.Infrastructure.ProfileLoad, new ProfileChannel() )
        {
            AddParam(UserId);
            AddParam(DeviceId);
            AddParam(GoogleId);
            AddParam(AppleId);
            AddParam(FacebookId);
            AddParam(AutoCreate);
        }
    }
}