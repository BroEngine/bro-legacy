using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public interface IServerCore
    {
        void Start(NetworkEngine.Config networkConfig, ContextStorage contextStorage);
        void Stop();
    }
}