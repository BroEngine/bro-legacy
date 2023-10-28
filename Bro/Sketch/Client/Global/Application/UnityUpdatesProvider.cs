using System;
using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class UnityUpdatesProvider : MonoBehaviour, Timing.IUpdatesProvider
    {
        public event Timing.Handler OnUpdate;
        public event Timing.Handler OnFixedUpdate;
        public event Timing.Handler OnLateUpdate;
    
        public event Action<bool> OnPause;
        public event Action<bool> OnFocus;

        float Timing.IUpdatesProvider.FixedUpdateInterval => Time.fixedDeltaTime;

        public static UnityUpdatesProvider Create(MonoBehaviour monoBehaviour)
        {
            var result = monoBehaviour.gameObject.AddComponent(typeof(UnityUpdatesProvider));
            return (UnityUpdatesProvider) result;
        }
        
        void Timing.IUpdatesProvider.RegisterUpdate(Timing.Handler h)
        {
            OnUpdate += h;
        }

        void Timing.IUpdatesProvider.UnregisterUpdate(Timing.Handler h)
        {
            OnUpdate -= h;
        }

        void Timing.IUpdatesProvider.RegisterFixedUpdate(Timing.Handler h)
        {
            OnFixedUpdate += h;
        }

        void Timing.IUpdatesProvider.UnregisterFixedUpdate(Timing.Handler h)
        {
            OnFixedUpdate -= h;
        }

        void Timing.IUpdatesProvider.RegisterLateUpdate(Timing.Handler h)
        {
            OnLateUpdate += h;
        }

        void Timing.IUpdatesProvider.UnregisterLateUpdate(Timing.Handler h)
        {
            OnLateUpdate -= h;
        }

        void Start()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }

        void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
        }

        void LateUpdate()
        {
            OnLateUpdate?.Invoke(Time.deltaTime);
        }

        private void OnApplicationQuit()
        {
            AssemblyProcessExit.InvokeProcessExit(this, null);
        }

        private void OnApplicationFocus(bool focus)
        {
            OnFocus?.Invoke(focus);
            new ApplicationFocusEvent(focus).Launch();
        }

        private void OnApplicationPause(bool pause)
        {
            OnPause?.Invoke(pause);
            new ApplicationPauseEvent(pause).Launch();
        }

        void OnDestroy()
        {
            OnUpdate = null;
            OnFixedUpdate = null;
            OnLateUpdate = null;
        }
    }
}