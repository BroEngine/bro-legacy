using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistrySetServerRequest  : ServiceRequest<RegistrySetServerRequest>
    {
        public readonly GameServerParam GameServer = new GameServerParam();
        
        public RegistrySetServerRequest() : base(Network.Request.OperationCode.Infrastructure.RegistrySet, new RegistryChannel() )
        {
            AddParam(GameServer);
        }
    }
}




