using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class NetworkModule : IClientContextModule
    {
        private IClientContext _context;
        public readonly NetworkEngine NetworkEngine = new NetworkEngine();
        
        void IClientContextModule.Initialize(IClientContext context)
        {
            _context = context;
        }
        
        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        IEnumerator IClientContextModule.Load()
        {
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            return null;
        }

    }
}