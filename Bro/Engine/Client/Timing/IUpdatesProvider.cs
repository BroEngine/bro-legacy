using System;

namespace Bro.Client
{
    public partial class Timing
    {
        public interface IUpdatesProvider
        {
            event Handler OnUpdate;
            event Handler OnFixedUpdate;
            event Handler OnLateUpdate;
           
            float FixedUpdateInterval { get; }
            
            void RegisterUpdate(Handler h);
            void UnregisterUpdate(Handler h);

            void RegisterFixedUpdate(Handler h);
            void UnregisterFixedUpdate(Handler h);

            void RegisterLateUpdate(Handler h);
            void UnregisterLateUpdate(Handler h);
        }
    }
}