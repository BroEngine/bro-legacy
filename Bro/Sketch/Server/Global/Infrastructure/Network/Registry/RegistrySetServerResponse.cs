using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistrySetServerResponse : ServiceResponse<RegistrySetServerResponse>
    {
        public readonly IntParam ServerId = new IntParam();
        
        public RegistrySetServerResponse() : base(Network.Request.OperationCode.Infrastructure.RegistrySet, new RegistryChannel() )
        {
            AddParam(ServerId);
        }
    }
}



        
        
