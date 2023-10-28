using System.Linq;
using Bro.Server;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Server.Network.Offline;

namespace Bro.Sketch.Server
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
    public class OfflineCore : IServerCore
    {
        private OfflineNetworkEngine _engine;

        void IServerCore.Start(NetworkEngine.Config networkConfig, ContextStorage contextStorage)
        {
            Bro.Log.Info("offline instance :: started");
            contextStorage.Initialize();

            var limbContexts = contextStorage.GetContexts<LimbContext>().ToArray<IServerContext>();
            var peerFactory = new ClientPeerFactory(limbContexts);

            _engine = new OfflineNetworkEngine(peerFactory);
        }

        void IServerCore.Stop()
        {
            Bro.Log.Info("offline instance :: stoped");
            _engine.Stop();
            _engine = null;
        }
    }
#endif
}