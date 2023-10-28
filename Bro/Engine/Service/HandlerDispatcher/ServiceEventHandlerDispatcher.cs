using System;
using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Server;
using Bro.Service;

namespace Bro.Service.Context
{
    public class ServiceEventHandlerDispatcher : BaseHandlerDispatcher
    {
        private delegate void RequestHandler(IServiceEvent request);
        private readonly IDictionary<byte, RequestHandler> _requestHandlers = new Dictionary<byte, RequestHandler>();
        
        public bool AttachHandlersToObject(object target)
        {
            var attached = false;
            var targetType = target.GetType();
            var objectMethodsData = GetObjectMethodsData(targetType);
            foreach (var hierarchyLevel in objectMethodsData.HierarchyData)
            {
                foreach (var handlerData in hierarchyLevel.Value)
                {
                    var cachedAttributeType = handlerData.AttributeType;
                    var cachedMethod = handlerData.Method;
                    var cachedAttribute = handlerData.Attribute;
                    if (cachedAttributeType == typeof(ServiceEventHandlerAttribute))
                    {
                        var requestHandlerAttribute = cachedAttribute as ServiceEventHandlerAttribute;
                        var requestHandler = (RequestHandler) Delegate.CreateDelegate(typeof(RequestHandler), target, cachedMethod);
                        if (requestHandlerAttribute != null && _requestHandlers.ContainsKey(requestHandlerAttribute.OperationCode))
                        {
                            Bro.Log.Error("OperationCode = " + requestHandlerAttribute.OperationCode + " already registered. Target =" + target.ToString());
                        }

                        if (requestHandlerAttribute != null)
                        {
                            _requestHandlers.Add(requestHandlerAttribute.OperationCode, requestHandler);
                            attached = true;
                        }
                    }
                }
            }

            return attached;
        }
        
        public void ProcessHandleEvent(IServiceEvent serviceEvent)
        {
            if (_requestHandlers.ContainsKey( serviceEvent.OperationCode ))
            {
                try
                {
                    var handler = _requestHandlers[serviceEvent.OperationCode];
                    handler.Invoke(serviceEvent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    Log.Error(ex.StackTrace);
                }
            }
        }
    }
}