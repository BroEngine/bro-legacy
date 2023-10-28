using System;
using System.Collections.Generic;
using System.Reflection;
using Bro.Network;
using Bro.Network.Service;
using Bro.Network.Web;
using Bro.Server.Network;
using Bro.Service;
using Bro.Service.Context;


namespace Bro.Server.Context
{
    public class ServerContext : IServerContext, IServiceDispatcher
    {
        private delegate void OnPeerJoinedHandler(IClientPeer peer, object data);
        private delegate void OnPeerLeftHandler(IClientPeer peer);
        private delegate void OnPeerDisconnectedHandler(IClientPeer peer);

        private event Action OnStartContext;
        private event Action OnStopContext;
        private event Action OnTerminateContext;
        private event OnPeerJoinedHandler OnPeerJoined;
        private event OnPeerLeftHandler OnPeerLeft;
        private event OnPeerDisconnectedHandler OnPeerDisconnected;

        private readonly IDictionary<int, IClientPeer> _peers = new Dictionary<int, IClientPeer>();
        
        private readonly CustomHandlerDispatcher _customHandlerDispatcher;
        private readonly RequestHandlerDispatcher _requestHandlerDispatcher;
        private readonly ServiceRequestHandlerDispatcher _serviceRequestHandlerDispatcher;
        private readonly ServiceEventHandlerDispatcher _serviceEventHandlerDispatcher;
        private readonly UpdateHandlerDispatcher _updateHandlerDispatcher;
        private readonly IList<IServerContextModule> _modules;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly Scheduler _scheduler;
        private readonly TaskContext _taskContext = new TaskContext();
        public int PeersAmount => _peers.Count;
        private bool _isInitialized;
        
        public ContextStorage ContextStorage { get; }
        public ConfigStorageCollector ConfigStorageCollector { get; }
        public IScheduler Scheduler => _scheduler;

        protected ServerContext(ContextStorage contextStorage, ConfigStorageCollector configStorageCollector)
        {   
            _customHandlerDispatcher = new CustomHandlerDispatcher();
            _requestHandlerDispatcher = new RequestHandlerDispatcher(GetType().ToString());
            _serviceRequestHandlerDispatcher = new ServiceRequestHandlerDispatcher();
            _serviceEventHandlerDispatcher = new ServiceEventHandlerDispatcher();
            _updateHandlerDispatcher = new UpdateHandlerDispatcher();
            _modules = FindModules();
            _scheduler = new Scheduler(GetType().Name);

            ContextStorage = contextStorage;
            if (ContextStorage == null)
            {
                Bro.Log.Info($"context {GetType()} created with no contextStorage, may lead to leakage");
            }

            ConfigStorageCollector = configStorageCollector;
        }

        private List<IServerContextModule> FindModules()
        {
            var result = new List<IServerContextModule>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var hierarchy = GetType().GetHierarchy();
            var iContextModulesName = typeof(IServerContextModule).Name;
            foreach (var type in hierarchy)
            {
                FieldInfo[] fields = type.GetFields(bindingFlags);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(IServerContextModule) || field.FieldType.GetInterface(iContextModulesName) != null)
                    {
                        var module = (IServerContextModule) field.GetValue(this);
                        result.Add(module);
                    }
                }
            }
            return result;
        }

        public void AddModule(IServerContextModule module)
        {
            _modules.Add(module);
            if (_isInitialized)
            {
                AttachModule(module);
            }
        }

        private void AttachModule(IServerContextModule module)
        {
            module.Initialize(this);
            _customHandlerDispatcher.ProcessCustomHandlers(module, module.Handlers);
            _requestHandlerDispatcher.AttachHandlersToObject(module);
            _updateHandlerDispatcher.AttachHandlersToObject(module, Scheduler);

            if (_serviceRequestHandlerDispatcher.AttachHandlersToObject(module))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }

            if (_serviceEventHandlerDispatcher.AttachHandlersToObject(module))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }
        }

        public void AttachHandlers(object o)
        {
            _customHandlerDispatcher.AttachCurrentObjectToAllHandlers(o);
            _requestHandlerDispatcher.AttachHandlersToObject(o);
            _updateHandlerDispatcher.AttachHandlersToObject(o, Scheduler);
        }
        
        IList<CustomHandlerDispatcher.HandlerInfo> GetBaseHandlers()
        {
            return new List<CustomHandlerDispatcher.HandlerInfo>()
            {
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(StartContextHandlerAttribute),
                    AttachHandler = d => OnStartContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(StopContextHandlerAttribute),
                    AttachHandler = d => OnStopContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(TerminateContextHandlerAttribute),
                    AttachHandler = d => OnTerminateContext += (Action) d,
                    HandlerType = typeof(Action)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(PeerJoinedHandlerAttribute),
                    AttachHandler = d => OnPeerJoined += (OnPeerJoinedHandler) d,
                    HandlerType = typeof(OnPeerJoinedHandler)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(PeerLeftHandlerAttribute),
                    AttachHandler = d => OnPeerLeft += (OnPeerLeftHandler) d,
                    HandlerType = typeof(OnPeerLeftHandler)
                },
                new CustomHandlerDispatcher.HandlerInfo()
                {
                    AttributeType = typeof(PeerDisconnectedHandlerAttribute),
                    AttachHandler = d => OnPeerDisconnected += (OnPeerDisconnectedHandler) d,
                    HandlerType = typeof(OnPeerDisconnectedHandler)
                }
            };
        }

        #region IContext Implementation

        public void Initialize()
        {
            _customHandlerDispatcher.AddHandlers(GetBaseHandlers());
            ContextStorage.OnContextInitialization(this);
            _customHandlerDispatcher.AttachCurrentObjectToAllHandlers(this);
            _requestHandlerDispatcher.AttachHandlersToObject(this);

            if (_serviceRequestHandlerDispatcher.AttachHandlersToObject(this))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }

            if (_serviceEventHandlerDispatcher.AttachHandlersToObject(this))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }

            _updateHandlerDispatcher.AttachHandlersToObject(this, Scheduler);

            foreach (var module in _modules)
            {
                AttachModule(module);
            }

            _isInitialized = true;
        }

        void IServiceDispatcher.HandleServiceRequest(IServiceRequest serviceRequest)
        {
            Scheduler.Schedule(() =>
            {
                try
                {
                    _serviceRequestHandlerDispatcher.ProcessHandleRequest(serviceRequest);
                }
                catch (Exception e)
                {
                    Bro.Log.Error($"server context :: exception during handling service request {e}");
                }
            });
        }

        void IServiceDispatcher.HandleServiceEvent(IServiceEvent serviceEvent)
        {
            Scheduler.Schedule(() =>
            {
                try
                {
                    _serviceEventHandlerDispatcher.ProcessHandleEvent(serviceEvent);
                }
                catch (Exception e)
                {
                    Bro.Log.Error($"server context :: exception during handling service event {e}");
                }
            });
        }

        void IServerContext.HandleRequest(INetworkRequest request, IClientPeer peer, Action onComplete)
        {
            Scheduler.Schedule(() =>
            {
                try
                {
                    _requestHandlerDispatcher.ProcessHandleRequest(request, peer);
                }
                catch (Exception e)
                {
                    Bro.Log.Error($"server context :: Exception during handling server request {e}");
                }
                onComplete?.Invoke();
            });
        }

        public void ForEachPeer(Action<IClientPeer> action)
        {
            foreach (var p in _peers)
            {
                if (!p.Value.Destroying)
                {
                    action(p.Value);
                }
            }
        }
        
        public IClientPeer GetPeer(Predicate<IClientPeer> predicate)
        {
            foreach (var p in _peers)
            {
                if (!p.Value.Destroying)
                {
                    if (predicate(p.Value))
                    {
                        return p.Value;
                    }
                }
            }

            return null;
        }

        public bool Join(IClientPeer peer, Action onJoinCompleted = null, object data = null)
        {
            bool canPeerJoin = peer.OnStartSwitchingContext();
            if (canPeerJoin)
            {
                Scheduler.Schedule(() =>
                {
                    try
                    {
                        ProcessJoin(peer, onJoinCompleted, data);
                    }
                    catch (Exception e)
                    {
                        Bro.Log.Error($"server context :: exception during peer join {e}");
                    }
                });
            }

            return canPeerJoin;
        }

        private void ProcessJoin(IClientPeer peer, System.Action onJoinCompleted, object data)
        {
            Action onLeaveCompleted = () =>
            {
                Action callback = () =>
                {
                    if (peer.Destroying)
                    {
                        Bro.Log.Info("server context :: player disconnected during the context change, nothing terrible");
                        return;
                    }

                    peer.Context = this;

                    _peers.Add(peer.PeerId, peer);

                    peer.OnFinishSwitchingContext();

                    OnPeerJoined?.Invoke(peer, data);
                    onJoinCompleted?.Invoke();
                };
                Scheduler.Schedule(callback);
            };

            var prevContext = peer.Context;
            if (prevContext != null)
            {
                prevContext.Leave(peer, onLeaveCompleted);
            }
            else
            {
                onLeaveCompleted();
            }
        }

        void IServerContext.Leave(IClientPeer peer, Action onLeaveCompleted)
        {
            Scheduler.Schedule(() => { ProcessLeave(peer, onLeaveCompleted); });
            //ProcessLeave(peer, onLeaveCompleted);
        }

        private void ProcessLeave(IClientPeer peer, Action onLeaveCompleted)
        {
            try
            {
                peer.Context = null;
                _peers.Remove(peer.PeerId);
                
                // Bro.Log.Info("server context :: peer id " + peer.PeerId + " on peer left invocation context = " + this.GetType());
                OnPeerLeft?.Invoke(peer);

                onLeaveCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Bro.Log.Error("server context :: exception during leave " + e);
            }
        }

        void IServerContext.OnDisconnect(IClientPeer peer, Action onDisconnectCompleted)
        {
            Scheduler.Schedule(() =>
            {
                ProcessOnDisconnect(peer);
                onDisconnectCompleted();
            });
        }

        private void ProcessOnDisconnect(IClientPeer peer)
        {
            ProcessLeave(peer, null);
            
            Bro.Log.Info("server context :: peer id " + peer.PeerId + " on peer disconnected invocation");
            OnPeerDisconnected?.Invoke(peer);
        }

        void IServerContext.Start()
        {
            ContextStorage?.OnContextStarted(this);
            _scheduler.Start();
            OnStartContext?.Invoke();
        }

        public void Stop()
        {
            _taskContext.Dispose(); 
            ContextStorage?.OnContextStopped(this);
            _scheduler.Stop();
            
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
            
            OnStopContext?.Invoke();
        }

        public void Terminate()
        {
            if (BrokerEngine.Instance.IsStarted)
            {
                BrokerEngine.Instance.RemoveDispatcher(this);
            }

            Stop();

            ForEachPeer(p => p.Disconnect(DisconnectCode.ContextTerminate));

            OnTerminateContext?.Invoke();
        }

        public T GetModule<T>() where T : class, IServerContextModule
        {
            T result = null;
            for (int i = 0, max = _modules.Count; i < max; ++i)
            {
                result = _modules[i] as T;
                if (result != null)
                {
                    break;
                }
            }

            if (result == null)
            {
                // Bro.Log.Error("no component of type = " + typeof(T));
            }

            return result;
        }

        public IServerContextModule GetModule(Predicate<IServerContextModule> matchPredicate)
        {
            IServerContextModule result = null;
            for (int i = 0, max = _modules.Count; i < max; ++i)
            {
                if (matchPredicate(_modules[i]))
                {
                    result = _modules[i];
                    break;
                }
            }

            return result;
        }

       

        

        #endregion

        #region Request Handling

        const long UpdateResponsesPeriod = 500L;

        [UpdateHandler(updatePeriod: UpdateResponsesPeriod)]
        private void UpdateResponses()
        {
            _requestHandlerDispatcher.UpdateResponse(UpdateResponsesPeriod);
            _serviceRequestHandlerDispatcher.UpdateResponse(UpdateResponsesPeriod);
        }

        #endregion


        public void SendServiceEvent(IServiceEvent serviceEvent)
        {
            if (IsServiceUsable)
            {
                BrokerEngine.Instance.Send(serviceEvent);
            }
        }

        public void SendServiceRequest(IServiceRequest serviceRequest)
        {
            SendServiceRequest(serviceRequest, null);
        }

        public void SendServiceRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback)
        {
            if (!IsServiceUsable)
            {
                callback?.Invoke(null);
                return;
            }

            // return to the calling thread
            BrokerEngine.Instance.SendRequest(serviceRequest, (mqcallback) =>
            {
                if (callback != null)
                {
                    Scheduler.Schedule(() => { callback(mqcallback); });
                }
            });
        }

        public bool IsServiceUsable => BrokerEngine.Instance.IsStarted && BrokerEngine.Instance.IsConnected;

        public IServiceChannel PrivateChannel => BrokerEngine.Instance.PrivateChannel;
        public IWebClient GetWebClient(bool keepAlive = false, long timeout = 5000)
        {
            return new WebClient(keepAlive, (int) timeout);
        }

        void ITaskContext.Add(Task task)
        {
            _taskContext.Add(task);
        }

        void ITaskContext.Remove(Task task)
        {
            _taskContext.Remove(task);
        }
        
        public void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
}