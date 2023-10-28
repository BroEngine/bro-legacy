using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bro.Threading;

namespace Bro.Client
{
    public partial class Timing
    {
        public class SystemUpdatesProvider : IUpdatesProvider
        {
            private const int UpdateInterval = 33;

            private static readonly List<SystemUpdatesProvider> _registeredUpdated = new List<SystemUpdatesProvider>();

            public event Timing.Handler OnUpdate;
            public event Timing.Handler OnFixedUpdate;
            public event Timing.Handler OnLateUpdate;
            
            private readonly Stopwatch _updateTimer;

            private bool _working;

            public SystemUpdatesProvider()
            {
                _registeredUpdated.Add(this);
                _updateTimer = new Stopwatch();
                _updateTimer.Start();
                _working = true;

                var thread = new BroThread(ThreadLogic) {IsBackground = true};
                thread.Start();
            }

            ~SystemUpdatesProvider()
            {
                _registeredUpdated.Remove(this);
            }

            private void Terminate()
            {
                _working = false;
            }

            private void ThreadLogic()
            {
                while (_working)
                {
                    var delta = (int) _updateTimer.ElapsedMilliseconds;
                    _updateTimer.Reset();
                    _updateTimer.Start();

                    if (OnUpdate != null)
                    {
                        OnUpdate.Invoke(delta);
                    }

                    if (OnFixedUpdate != null)
                    {
                        OnFixedUpdate.Invoke(delta);
                    }

                    if (OnLateUpdate != null)
                    {
                        OnLateUpdate.Invoke(delta);
                    }

                    Thread.Sleep(UpdateInterval);
                }
            }

            float IUpdatesProvider.FixedUpdateInterval => UpdateInterval * 0.0001f;

           

            void IUpdatesProvider.RegisterUpdate(Handler h)
            {
                OnUpdate += h;
            }

            void IUpdatesProvider.UnregisterUpdate(Handler h)
            {
                OnUpdate -= h;
            }

            void IUpdatesProvider.RegisterFixedUpdate(Handler h)
            {
                OnFixedUpdate += h;
            }

            void IUpdatesProvider.UnregisterFixedUpdate(Handler h)
            {
                OnFixedUpdate -= h;
            }

            void IUpdatesProvider.RegisterLateUpdate(Handler h)
            {
                OnLateUpdate += h;
            }

            void IUpdatesProvider.UnregisterLateUpdate(Handler h)
            {
                OnLateUpdate -= h;
            }
            
            public static void TerminateAll()
            {
                var hitList = _registeredUpdated.ToArray();
                _registeredUpdated.Clear();

                for (var i = 0; i < hitList.Length; ++i)
                {
                    hitList[i].Terminate();
                }
            }
        }
    }
}