using System.Collections.Generic;
using Bro.Network;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class AuthorizationResponse : NetworkResponse<AuthorizationResponse>
    {
        public readonly BoolParam IsAuthorized = new BoolParam();
        public readonly ByteParam ErrorCode = new ByteParam();
        public readonly BoolParam Queued = new BoolParam();
        public readonly IntParam UserId = new IntParam(isOptional:true);
        public readonly ArrayParam<ConfigDetailsParam> ConfigDetailsArray = new ArrayParam<ConfigDetailsParam>(byte.MaxValue);
        
        public AuthorizationResponse() : base(Request.OperationCode.Authorization)
        {
            AddParam(IsAuthorized);
            AddParam(Queued);
            AddParam(UserId);
            AddParam(ErrorCode);
            AddParam(ConfigDetailsArray);
        }

        public void SetConfigDetails(IEnumerable<ConfigDetails> values)
        {
            foreach (var configDetails in values)
            {
                var detailsParam = NetworkPool.GetParam<ConfigDetailsParam>();
                detailsParam.Value = configDetails;
                ConfigDetailsArray.Params.Add(detailsParam);
            }
        }
    }
}