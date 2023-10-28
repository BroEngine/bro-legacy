using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public static class ClientContextExtension
    {
        public static NetworkEngine GetNetworkEngine(this IClientContext context)
        {
            var result = context.GetModule<NetworkModule>()?.NetworkEngine;
            if (result == null)
            {
                result = context.Application.GlobalContext.GetModule<NetworkModule>().NetworkEngine;
            }
            return result;
        }
        
        public static ClientApplication GetApplication(this IClientContext clientContext)
        {
            return clientContext.Application as ClientApplication;
        }
    }
}