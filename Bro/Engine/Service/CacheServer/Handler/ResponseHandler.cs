using System.Collections.Generic;
using System.Threading;
using Bro.Threading;

namespace Bro.Service.Cache
{
    public class ResponseHandler
    {
        private const int ThreadMandatorySleep = 100;
        private const int Timeout = 1000;
       
        private readonly object _lock = new object();
        private BroThread _thread;
    
        private readonly Dictionary<IOperation,long> _pendingOperations = new Dictionary<IOperation, long>();
    
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
        
        public void RegisterRequest( IOperation operation )
        {
            lock (_lock)
            {
                _pendingOperations.Remove(operation);
                _pendingOperations.Add(operation, TimeInfo.GlobalTimestamp);
            }
        }

        public void OnResponse(IOperation operation)
        {
            lock (_lock)
            {
                if (_pendingOperations.ContainsKey(operation))
                {
                    operation.InvokeCallback(true);
                    _pendingOperations.Remove(operation);
                }
            }
        }
        
        public void Reset()
        {
            lock (_lock)
            {
                foreach (var operation in _pendingOperations)
                {
                    operation.Key.InvokeCallback(false);
                }
                
                _pendingOperations.Clear();
            }
        }

        private void HandleTimeout()
        {
            lock (_lock)
            {
                var timeouted = new List<IOperation>();
                foreach (var operationPair in _pendingOperations)
                {
                    var time = operationPair.Value;
                    if (TimeInfo.GlobalTimestamp - time > Timeout)
                    {
                        timeouted.Add(operationPair.Key);
                    }
                }

                foreach (var operation in timeouted)
                {
                    operation.InvokeCallback(false);
                    _pendingOperations.Remove(operation);
                }
            }
        }

    }
}