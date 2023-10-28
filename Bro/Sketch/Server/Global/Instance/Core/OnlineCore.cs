using System.Linq;
using Bro.Server;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Threading;

namespace Bro.Sketch.Server
{
    public class OnlineCore : IServerCore
    {
        private NetworkEngine _engine;
        private BroThread _thread;

        void IServerCore.Start(NetworkEngine.Config networkConfig, ContextStorage contextStorage)
        {
            contextStorage.Initialize();
            var limbContexts = contextStorage.GetContexts<LimbContext>().ToArray<IServerContext>();
            Bro.Log.Info("limbContexts "  +limbContexts.Length + " contextStorage " + contextStorage.GetType());
            ClientPeerFactory peerFactory = new ClientPeerFactory(limbContexts);
            _thread = new BroThread(() => StartNetworkEngine(peerFactory, networkConfig ));
            _thread.Start();
        }

        void IServerCore.Stop()
        {
            _engine.Stop();
            _engine = null;
            _thread.Abort();
            _thread = null;
        }

        private void StartNetworkEngine(INetworkPeerFactory peerFactory, NetworkEngine.Config config)
        {
            _engine = new NetworkEngine(peerFactory, config);
        }
    }
}