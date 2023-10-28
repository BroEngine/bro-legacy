using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bro.Client.Context;
using Bro.Network;

namespace Bro.Client
{
    public class ClientContext : IClientContext
    {
        private event Action OnStartLoadContext;
        private event Action OnFinishLoadContext;
        private event Action OnStartUnloadContext;
        private event Action OnFinishUnloadContext;

        private ClientApplication _application;
        private Timing.IScheduler _scheduler;

        private readonly UpdateFrameHandlerDispatcher _updateHandlerDispatcher = new UpdateFrameHandlerDispatcher();
        private readonly CustomHandlerDispatcher _customHandlerDispatcher = new CustomHandlerDispatcher();
        private IList<IClientContextModule> _modules;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly TaskContext _taskContext = new TaskContext();

        public Timing.IScheduler Scheduler => _scheduler;
        
        public bool IsAlive { get; private set; }

        public ClientApplication Application
        {
            get
            {
                if (_application != null)
                {
                    return _application;
                }
                Bro.Log.Error("client context :: trying to get the application from the uploaded context");
                return null;
            }
        }

        IList<CustomHandlerDispatcher.HandlerInfo> GetBaseHandlers()
        {
            return new List<CustomHandlerDispatcher.HandlerInfo>()
            {
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(StartLoadContextHandlerAttribute),
                    AttachHandler = d => OnStartLoadContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(FinishLoadContextHandlerAttribute),
                    AttachHandler = d => OnFinishLoadContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(StartUnloadContextHandlerAttribute),
                    AttachHandler = d => OnStartUnloadContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(FinishUnloadContextHandlerAttribute),
                    AttachHandler = d => OnFinishUnloadContext += (Action) d,
                    HandlerType = typeof(Action)
                }
            };
        }
        private IList<IClientContextModule> FindModules()
        {
            var result = new List<IClientContextModule>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var hierarchy = GetType().GetHierarchy();
            var iContextModuleName = typeof(IClientContextModule).Name;
            foreach (var type in hierarchy)
            {
                var fields = type.GetFields(bindingFlags);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(IClientContextModule) || field.FieldType.GetInterface(iContextModuleName) != null)
                    {
                        var module = (IClientContextModule) field.GetValue(this);
                        result.Add(module);
                    }
                }
            }
            return result;
        }

        private void AttachModule(IClientContextModule module)
        {
            module.Initialize(this);
            _customHandlerDispatcher.ProcessCustomHandlers(module, module.Handlers);
            _updateHandlerDispatcher.AttachHandlersToObject(module, _scheduler);
        }

        public T GetModule<T>() where T : class, IClientContextModule
        {
            T result = null;
            if (_modules != null)
            {
                for (int i = 0, max = _modules.Count; i < max; ++i)
                {
                    result = _modules[i] as T;
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            
            #if UNITY_EDITOR
            if (result == null && this != Application.GlobalContext)
            {
                if (Application.GlobalContext.GetModule<T>() != null)
                {
                    if (typeof(T) != typeof(Bro.Sketch.Client.NetworkModule))
                    {
                        Bro.Log.Error("module of type = " + typeof(T) + " placed in the global context, not local");
                    }
                }
                else
                {
                    Bro.Log.Error("no module of type = " + typeof(T) + " in the context = " + this.GetType());
                }
            }
            #endif
            
            return result;
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        void IClientContext.Load(ClientApplication clientApplication, Action onFinish)
        {
            IsAlive = true;
            _application = clientApplication;
            _scheduler = new Timing.Scheduler(this);

            OnStartLoadContext?.Invoke();

            _customHandlerDispatcher.AddHandlers(GetBaseHandlers());
            _customHandlerDispatcher.AttachCurrentObjectToAllHandlers(this);

            _scheduler.StartCoroutine(LoadModules(onFinish));
        }
        
        private IEnumerator LoadModules(Action onFinish)
        {
            _modules = FindModules();
            foreach (var module in _modules)
            {
                AttachModule(module);
            }
            
            foreach (var module in _modules)
            {
                yield return module.Load();
            }
            
            OnFinishLoadContext?.Invoke();
            onFinish?.Invoke();
            
            new ContextLoadedEvent(this).Launch();
        }

        public void ResetScheduler()
        {
            _scheduler.Dispose();
            _scheduler = new Timing.Scheduler(this);
        }

        void IClientContext.Unload(Action onFinish)
        {
            OnStartUnloadContext?.Invoke();
            
            /* recreating scheduler, to stop all current update and schedule unload process */
            ResetScheduler();
            
            _scheduler.StartCoroutine(ProcessUnload(onFinish));
            _customHandlerDispatcher.Clear();
        }

        private IEnumerator ProcessUnload(Action onFinish)
        {
            foreach (var module in _modules)
            {
                yield return module.Unload();
            }
            _modules = null;
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
            onFinish?.Invoke();
            OnFinishUnloadContext?.Invoke();
            IsAlive = false;
            
            new ContextUnloadedEvent(this).Launch();
            _scheduler.Dispose();
            _taskContext.Dispose();
        }

        public IWebClient GetWebClient(bool keepAlive = false, long timeout = 5000)
        {        
            #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            return new UnityWebClient(this, timeout);
            #else
            return new Bro.Network.Web.WebClient(keepAlive: false, timeout: timeout);
            #endif
        }

        void ITaskContext.Add(Task task)
        {
            _taskContext.Add(task);
        }

        void ITaskContext.Remove(Task task)
        {
            _taskContext.Remove(task);
        }
    }
}