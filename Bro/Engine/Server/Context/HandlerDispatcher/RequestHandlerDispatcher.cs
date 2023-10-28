using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro
{
    public class RequestHandlerDispatcher : BaseHandlerDispatcher
    {
        private delegate INetworkResponse RequestHandler(INetworkRequest request, IClientPeer peer);
        private readonly IDictionary<byte, RequestHandler> _requestHandlers = new Dictionary<byte, RequestHandler>();
        private readonly string _ownerInfo; 
        private readonly List<string> _handlerNames = new List<string>();

        public RequestHandlerDispatcher(string ownerInfo)
        {
            _ownerInfo = ownerInfo;
        }
        
        public void AttachHandlersToObject(object target)
        {
            _handlerNames.Clear();
            var targetType = target.GetType();
            var objectMethodsData = GetObjectMethodsData(targetType);
            foreach (var hierarchyLevel in objectMethodsData.HierarchyData)
            {
                foreach (var handlerData in hierarchyLevel.Value)
                {
                    var cachedAttributeType = handlerData.AttributeType;
                    var cachedMethod = handlerData.Method;
                    var cachedAttribute = handlerData.Attribute;
                    if (cachedAttributeType == typeof(RequestHandlerAttribute))
                    {
                        if (_handlerNames.Contains(cachedMethod.Name))
                        {
                            Bro.Log.Error("Method is duplicated in class hierarchy. Method should be private " + cachedMethod.Name);
                        }
                        var requestHandlerAttribute = cachedAttribute as RequestHandlerAttribute;
                        var requestHandler = (RequestHandler) Delegate.CreateDelegate(typeof(RequestHandler), target, cachedMethod);
                        if (requestHandlerAttribute != null && _requestHandlers.ContainsKey(requestHandlerAttribute.OperationCode))
                        {
                            Bro.Log.Error("OperationCode = " + requestHandlerAttribute.OperationCode + " already registered. Target =" + target.ToString() + " owner = " + _ownerInfo);
                        }

                        if (requestHandlerAttribute != null)
                        {
                            _requestHandlers.Add(requestHandlerAttribute.OperationCode, requestHandler);
                            _handlerNames.Add(cachedMethod.Name);
                        }
                    }
                }
            }
        }
        
        class WaitingResponseData
        {
            public readonly IClientPeer peer;
            public readonly INetworkResponse response;
            private long _waitTimer;

            public WaitingResponseData(IClientPeer peer, INetworkResponse response, long waitTimer)
            {
                this.peer = peer;
                this.response = response;
                this._waitTimer = waitTimer;
            }

            /// <summary>
            /// Update the specified deltaTime.
            /// </summary>
            /// <returns>The update.</returns>
            /// <param name="deltaTime">Delta time in ms.</param>
            public bool Update(long deltaTime)
            {
                _waitTimer -= deltaTime;
                return _waitTimer <= 0L;
            }
        }

        private readonly IList<WaitingResponseData> _waitingResponses = new List<WaitingResponseData>();
        public void ProcessHandleRequest(INetworkRequest request, IClientPeer peer)
        {
            var point = PerformanceMeter.Register(PerformancePointType.ContextRequestHandler);
            
            var errorCode = Network.ErrorCode.NoError;
            if (!_requestHandlers.TryGetValue(request.OperationCode, out var handler))
            {
                errorCode = Network.ErrorCode.NoHandlerForOperationCode;
            }
            else if (!request.HasValidParams)
            {
                Log.Error("Invalid params in request with operation code = " + request.OperationCode + "; owner " + _ownerInfo);
                errorCode = Network.ErrorCode.InvalidParam;
            }

            if (errorCode == Network.ErrorCode.NoError)
            {
                if (!peer.Destroying)
                {
                    INetworkResponse response = null;
                    peer.OnStartHandleRequest();

                    try
                    {
                        if (handler != null)
                        {
                            response = handler.Invoke(request, peer);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Log.Error(ex.StackTrace);
                        errorCode = Network.ErrorCode.ServerError;
                    }

                    peer.OnEndHandleRequest();

                    if (response != null)
                    {
                        if (!response.IsHeld)
                        {
                            peer.Send(response);
                        }
                        else
                        {
                            response.Retain();
                            _waitingResponses.Add(new WaitingResponseData(peer, response, NetworkConfig.ResponseTimeout));
                        }
                    }
                }
            }

            if (errorCode != Network.ErrorCode.NoError)
            {
                if (errorCode == Network.ErrorCode.NoHandlerForOperationCode)
                {
                    // Bro.Log.Error("Context = " + _ownerInfo + " does not contain any handlers for request = " + request);
                    return;
                }
                else
                {
                    Bro.Log.Error("ProcessHandleRequest " + _ownerInfo + " request = " + request + " errorCode " + errorCode);
                }

                var response = NetworkOperationFactory.CreateResponse(request);
                if (response != null)
                {
                    response.ErrorCode = errorCode;
                    peer.Send(response);
                }
            }
            
            // request.Release();
            
            point?.Done();
        }

        public void UpdateResponse(long updateResponsesPeriod)
        {
            for (int i = 0; i < _waitingResponses.Count;)
            {
                var data = _waitingResponses[i];
                if (!data.response.IsHeld)
                {
                    data.peer.Send(data.response);
                    _waitingResponses.FastRemoveAtIndex(i);
                    data.response.Release();
                }
                else if (data.Update(updateResponsesPeriod))
                {
                    _waitingResponses.FastRemoveAtIndex(i);
                    data.response.Release();
                }
                else
                {
                    ++i;
                }
            }
        }
    }
}