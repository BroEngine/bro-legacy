using System;
using System.Collections.Generic;
using System.Reflection;
using Bro.Network;
using Bro.Network.Service;

namespace Bro.Service.Context
{
    public class ServiceContext : IServiceContext, IServiceDispatcher
    {
        private delegate void UpdateHandler();

        private readonly ServiceRequestHandlerDispatcher _serviceRequestHandlerDispatcher;
        private readonly ServiceEventHandlerDispatcher _serviceEventHandlerDispatcher;
        private readonly UpdateHandlerDispatcher _updateHandlerDispatcher;
        private readonly IList<IServiceContextComponent> _components = new List<IServiceContextComponent>();
        private readonly Scheduler _scheduler;

        public IScheduler Scheduler => _scheduler;
        
        public ServiceContext()
        {
            _scheduler = new Scheduler(GetType().Name);
            _serviceRequestHandlerDispatcher = new ServiceRequestHandlerDispatcher();
            _serviceEventHandlerDispatcher = new ServiceEventHandlerDispatcher();
            _updateHandlerDispatcher = new UpdateHandlerDispatcher();           
        }

        private void InitModules()
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var hierarchy = GetType().GetHierarchy();
            var iContextComponentName = typeof(IServiceContextComponent).Name;
            foreach (var type in hierarchy)
            {
                var fields = type.GetFields(bindingFlags);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(IServiceContextComponent) || field.FieldType.GetInterface(iContextComponentName) != null)
                    {
                        var component = (IServiceContextComponent) field.GetValue(this);
                        AddComponent(component);
                    }
                }
            }
        }

        private void AddComponent(IServiceContextComponent component)
        {
            component.Init(this);
            _components.Add(component);
            _updateHandlerDispatcher.AttachHandlersToObject(component,_scheduler);
            
            if (_serviceRequestHandlerDispatcher.AttachHandlersToObject(component))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }
            
            if (_serviceEventHandlerDispatcher.AttachHandlersToObject(component))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }
        }

        public void Initialize()
        {
            if (_serviceRequestHandlerDispatcher.AttachHandlersToObject(this))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }
            
            if (_serviceEventHandlerDispatcher.AttachHandlersToObject(this))
            {
                BrokerEngine.Instance.RegisterDispatcher(this);
            }
            
            _updateHandlerDispatcher.AttachHandlersToObject(this, _scheduler);
            InitModules();
        }
        
        public virtual void Start()
        {
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Stop();
        }

        public void HandleRequest(INetworkRequest request)
        {
            throw new NotImplementedException();
        }

        public void HandleServiceRequest(IServiceRequest serviceRequest)
        {
            Scheduler.Schedule(() =>
            {
                try
                {
                    _serviceRequestHandlerDispatcher.ProcessHandleRequest(serviceRequest);
                }
                catch (Exception e)
                {
                    Bro.Log.Error("[IMPORTANT] Exeption during IContext.HandleServiceRequest " + e);
                }
            });
        }

        public void HandleServiceEvent(IServiceEvent serviceEvent)
        {
            Scheduler.Schedule(() =>
            {
                try
                {
                    _serviceEventHandlerDispatcher.ProcessHandleEvent(serviceEvent);
                }
                catch (Exception e)
                {
                    Bro.Log.Error("[IMPORTANT] Exeption during IContext.HandleServiceEvent " + e);
                }
            });
        }
       
        public T GetComponent<T>() where T : class, IServiceContextComponent
        {
            T result = null;
            for (int i = 0, max = _components.Count; i < max; ++i)
            {
                result = _components[i] as T;
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        

        public void Terminate()
        {
            if (BrokerEngine.Instance.IsStarted)
            {
                BrokerEngine.Instance.RemoveDispatcher(this);
            }

            Stop();
        }

        const long UpdateResponsesPeriod = 500L;

        [UpdateHandler(updatePeriod: UpdateResponsesPeriod)]
        private void UpdateResponses()
        {
            _serviceRequestHandlerDispatcher.UpdateResponse(UpdateResponsesPeriod);
        }
        
        public void SendServiceEvent(IServiceEvent serviceEvent)
        {
            if (BrokerEngine.Instance.IsStarted)
            {
                BrokerEngine.Instance.Send(serviceEvent);
            }
        }

        public void SendServiceRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback)
        {
            if (!BrokerEngine.Instance.IsStarted)
            {
                callback(null);
                return;
            }

            // return to the calling thread
            BrokerEngine.Instance.SendRequest( serviceRequest, (mqcallback) =>
            {
                Scheduler.Schedule(() => { callback( mqcallback ); });
            });
        }
        
        public bool IsServiceUsable => BrokerEngine.Instance.IsConnected && BrokerEngine.Instance.IsStarted;

        public IServiceChannel PrivateChannel => BrokerEngine.Instance.PrivateChannel;
    }
}