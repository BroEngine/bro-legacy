using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class ClientApplication : Bro.Client.ClientApplication
    {
        private readonly Timing.IUpdatesProvider _updatesProvider;
        public override Timing.IUpdatesProvider UpdatesProvider => _updatesProvider;

        public ClientApplication()
        {
            #if CONSOLE_CLIENT
            _updatesProvider = new Timing.SystemUpdatesProvider();
            #else
            _updatesProvider = new GameObject("system").AddComponent<UnityUpdatesProvider>();
            #endif
        }

        public override void Start(IClientContext globalContext, IClientContext entryContext)
        {
            SetupLog();
            base.Start(globalContext, entryContext);
        }

        private static void SetupLog()
        {
            Bro.Log.PrintTimestamp = true;
         
            #if ! CONSOLE_CLIENT
            Bro.Log.UseCustomLog = true;

            Bro.Log.OnCustomLogInfo += UnityEngine.Debug.Log;
            Bro.Log.OnCustomLogWarning += UnityEngine.Debug.LogWarning;
            Bro.Log.OnCustomLogError += UnityEngine.Debug.LogError;
            #endif
        }
    }
}