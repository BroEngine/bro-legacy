using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Network.Service;
using Bro.Threading;

namespace Bro.Service
{
    public class ResponseOperation
    {
        private const int ThreadMandatorySleep = 100;
        private const int Timeout = 10000;
       
        private readonly object _lock = new object();
        private BroThread _thread;
        
        private static class RequestCounter
        {
            private static byte _counter = 0;

            public static byte Next { get { return ++_counter; } }
        }

        private class Callback
        {
            public readonly Action<IServiceResponse> Action;
            public readonly long Time;

            public Callback(Action<IServiceResponse> action)
            {
                Time = TimeInfo.GlobalTimestamp;
                Action = action;
            }
        }

        private readonly IServiceChannel _privateChannel;
        private readonly Dictionary<int,Callback> _pendingCallbacks = new Dictionary<int, Callback>();
        
        public ResponseOperation(IServiceChannel privateChannel)
        {
            _privateChannel = privateChannel;
        }

        public void Start()
        {
            _thread = new BroThread(WorkCycle);
            _thread.Start();
        }

        private void WorkCycle()
        {
            while (true)
            {
                HandleTimeout();
                Thread.Sleep(ThreadMandatorySleep);
            }
        }
        
        public void RegisterRequest( IServiceRequest serviceRequest, Action<IServiceResponse> callback )
        {
            serviceRequest.TemporaryIdentifier = RequestCounter.Next;
            serviceRequest.ResponseChannel = _privateChannel;
            
            var temporaryIdentifier = serviceRequest.TemporaryIdentifier;
            var operationCode = serviceRequest.OperationCode;
            var hash = GetOperationHash(temporaryIdentifier,operationCode);

            lock (_lock)
            {
                if (callback != null)
                {
                    _pendingCallbacks.Remove(hash);
                    _pendingCallbacks.Add(hash, new Callback(callback));
                }
            }
        }

        public void OnResponse(IServiceResponse serviceResponse)
        {
            var temporaryIdentifier = serviceResponse.TemporaryIdentifier;
            var operationCode = serviceResponse.OperationCode;
            var hash = GetOperationHash(temporaryIdentifier,operationCode);

            lock (_lock)
            {
                if (_pendingCallbacks.ContainsKey(hash))
                {
                    _pendingCallbacks[hash].Action(serviceResponse);
                    _pendingCallbacks.Remove(hash);
                }
            }
        }
        
        private int GetOperationHash(byte temporaryIdentifier, byte operationCode)
        {
            return operationCode * 1000 + temporaryIdentifier;
        }

        public void Reset()
        {
            lock (_lock)
            {
                foreach (var callback in _pendingCallbacks)
                {
                    var action = callback.Value.Action;
                    action(null);
                }
                
                _pendingCallbacks.Clear();
            }
        }

        private void HandleTimeout()
        {
            lock (_lock)
            {
                var timeouted = new List<int>();
                foreach (var callbackPair in _pendingCallbacks)
                {
                    var callback = callbackPair.Value;
                    var time = callback.Time;

                    if (TimeInfo.GlobalTimestamp - time > Timeout)
                    {
                        timeouted.Add(callbackPair.Key);
                    }
                }

                foreach (var hash in timeouted)
                {
                    var callback = _pendingCallbacks[hash];
                    callback.Action(null);
                    _pendingCallbacks.Remove(hash);
                }
            }
        }
    }
}