using System;
using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class ObjectBehaviour : MonoBehaviour, IObjectBehaviour
    {
        private IObjectElement[] _elements = null;
        
        public IClientContext Context { get; private set; }
        
        public event Action<float> OnUpdate;
        public event Action<float> OnFixedUpdate;
        
        private IDisposable _update;
        private IDisposable _fixedUpdate;
        
        public void Init(IClientContext context)
        {
            Context = context;
            
            var elements = GetElements();
            foreach (var element in elements)
            {
                element.SetBehaviour(this);
            }
        }

        public void Create()
        {
            var elements = GetElements();
            foreach (var element in elements)
            {
                element.Create();
            }
            
            _update = Context.Scheduler.ScheduleUpdate(ProcessUpdate); // optimize
            _fixedUpdate = Context.Scheduler.ScheduleUpdate(ProcessFixedUpdate); // optimize
        }
        
        public void Destroy()
        {   
            var elements = GetElements();
            foreach (var element in elements)
            {
                element.Destroy();
            }
        }

        public void OnPoolIn() // todo расписать на личточке всю схему подписок-отписок, жизненого цикла и тд - а потом переделать нормально!
        {
            _fixedUpdate?.Dispose();
            _fixedUpdate = null;
            
            _update?.Dispose();
            _update = null;
            
            OnUpdate = null;
            OnFixedUpdate = null;
            
            // OnDestroy = null; 
        }
        
        private void ProcessUpdate(float dt)
        {
            //UnityEngine.Profiling.Profiler.BeginSample($"beh_upd");
            OnUpdate?.Invoke(Time.deltaTime);
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        private void ProcessFixedUpdate(float dt)
        {
            //UnityEngine.Profiling.Profiler.BeginSample($"beh_f_upd");
            OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
            //UnityEngine.Profiling.Profiler.EndSample();
        }
        
        public T GetElement<T>() where T : IObjectElement
        {
            var behavior = gameObject.GetComponent<T>();
            if (behavior == null)
            {
                // Bro.Log.Error("no behaviour of type = " + typeof(T));
            }
            return behavior;
        }

        public IObjectElement[] GetElements()
        {
            if (_elements == null)
            {
                _elements = gameObject.GetComponentsInChildren<IObjectElement>();
            }

            return _elements;
        }

        public void OnPoolOut()
        {
            
        }

        public void ResetCache()
        {
            _elements = null;
        }
    }
}