using Bro.Network.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistryGetServersRequest : ServiceRequest<RegistryGetServersRequest>
    {
        public RegistryGetServersRequest() : base(Network.Request.OperationCode.Infrastructure.RegistryGet, new RegistryChannel() )
        {
            
        }
    }
}