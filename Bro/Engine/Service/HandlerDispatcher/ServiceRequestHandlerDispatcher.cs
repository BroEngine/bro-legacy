using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Network.Service;

namespace Bro.Service.Context
{
    public class ServiceRequestHandlerDispatcher : BaseHandlerDispatcher
    {
        private delegate IServiceResponse RequestHandler(IServiceRequest request);
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
                    if (cachedAttributeType == typeof(ServiceRequestHandlerAttribute))
                    {
                        var requestHandlerAttribute = cachedAttribute as ServiceRequestHandlerAttribute;
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
        
        private  class WaitingResponseData
        {
            public readonly IServiceResponse Response;
            private long _waitTimer;

            public WaitingResponseData(IServiceResponse response, long waitTimer)
            {
                Response = response;
                _waitTimer = waitTimer;
            }

            public bool Update(long deltaTime)
            {
                _waitTimer -= deltaTime;
                return _waitTimer <= 0L;
            }
        }

        private readonly IList<WaitingResponseData> _waitingResponses = new List<WaitingResponseData>();
        public void ProcessHandleRequest(IServiceRequest request)
        {
            IServiceResponse response = null;
            RequestHandler handler;
            if (!_requestHandlers.TryGetValue(request.OperationCode, out handler))
            {
                return;
            }
            
            try
            {
                if (handler != null)
                {
                    response = handler.Invoke( request );
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Log.Error(ex.StackTrace);
            }

            if (response != null)
            {
                if (!response.IsHolded)
                {
                    BrokerEngine.Instance.Send( response );
                }
                else
                {
                    _waitingResponses.Add( new WaitingResponseData( response, NetworkConfig.ResponseTimeout));
                }
            }
        }

        public void UpdateResponse(long updateResponsesPeriod)
        {
            for (int i = 0; i < _waitingResponses.Count;)
            {
                var data = _waitingResponses[i];
                if (!data.Response.IsHolded)
                {
                    BrokerEngine.Instance.Send( data.Response );
                    _waitingResponses.FastRemoveAtIndex(i);
                }
                else if (data.Update(updateResponsesPeriod))
                {
                    _waitingResponses.FastRemoveAtIndex(i);
                }
                else
                {
                    ++i;
                }
            }
        }
    }
}