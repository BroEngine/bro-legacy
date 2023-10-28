using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistryGetServersResponse : ServiceResponse<RegistryGetServersResponse>
    {
        public readonly ArrayParam<GameServerParam> Servers = new ArrayParam<GameServerParam>(byte.MaxValue);
        
        public RegistryGetServersResponse() : base(Network.Request.OperationCode.Infrastructure.RegistryGet, new RegistryChannel() )
        {
            AddParam(Servers);
        }
    }
}