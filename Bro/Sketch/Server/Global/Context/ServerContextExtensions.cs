using System.Runtime.CompilerServices;
using Bro.Server.Context;

namespace Bro.Sketch.Server
{
    public static class ServerContextExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Actor GetActor(this IServerContext context, int userId)
        {
            var peer = context.GetPeer(p => p.GetActor().Profile.UserId == userId);
            return peer?.GetActor();
        }
        
        public static ConfigHolder GetDefaultConfigHolder(this IServerContext context)
        {
            var configHolderProvider = context.GetModule(c => c is IConfigHolderProvider) as IConfigHolderProvider;
            if (configHolderProvider == null)
            {
                Bro.Log.Error("context extensions :: cannot find config holder provider in context");
                return null;
            }
            return configHolderProvider.CreateDefaultConfigHolder();
        }
    }
}