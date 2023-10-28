using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class AuthorizationRequest : NetworkRequest<AuthorizationRequest>
    {
        public readonly StringParam DeviceId = new StringParam();
        public readonly ByteParam Platform = new ByteParam();
        public readonly IntParam SessionId = new IntParam();

        public readonly StringParam GoogleId = new StringParam(isOptional:true);
        public readonly StringParam AppleId = new StringParam(isOptional:true);
        public readonly StringParam FacebookId = new StringParam(isOptional:true);
        
        public readonly BoolParam ResetProfile = new BoolParam();
        
        public AuthorizationRequest() : base(Request.OperationCode.Authorization)
        {
            AddParam(DeviceId);  
            AddParam(Platform);
            AddParam(SessionId);
            
            AddParam(GoogleId);
            AddParam(AppleId);
            AddParam(FacebookId);
            
            AddParam(ResetProfile);
        }
    }
}