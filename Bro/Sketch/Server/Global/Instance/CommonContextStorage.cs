using System;
using Bro.Server.Context;

namespace Bro.Sketch.Server
{
    public class CommonContextStorage : ContextStorage
    {
        public readonly ServerConfig ServerConfig;
        
        public ConfigStorageCollector ConfigStorageCollector;
        
        public CommonContextStorage(ServerConfig serverConfig)
        {
            ServerConfig = serverConfig;
        }
        
        public override void Initialize()
        {
            base.Initialize();

            if (ConfigStorageCollector == null)
            {
                throw new Exception("ConfigStorageCollector is not initialized");
            }
            
            IServerContext limbContext = new LimbContext(this,ConfigStorageCollector);
            limbContext.Initialize();
            limbContext.Start();
        }
    }
}