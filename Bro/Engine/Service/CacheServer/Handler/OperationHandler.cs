using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Threading;
using Sider;

namespace Bro.Service.Cache
{
    public class OperationHandler
    {
        private BroThread _thread;
        
        private readonly object _mutex = new object();

        private Queue<IOperation> _activeQueue = new Queue<IOperation>();
        private Queue<IOperation> _onholdQueue = new Queue<IOperation>();

        private readonly ResponseHandler _responseHandler;
        private readonly CacheServerEngine _engine;
        private readonly object _connectionLock;

        private RedisClient _client;
        
        public OperationHandler(CacheServerEngine engine, object connectionLock)
        {
            _responseHandler = new ResponseHandler();
            _engine = engine;
            _connectionLock = connectionLock;
        }

        public void Start()
        {
            _responseHandler.Start();
            
            _thread = new BroThread(WorkCycle);
            _thread.Start();
        }

        public void Reset()
        {
            _client = null;
            _responseHandler.Reset();
            
            lock (_mutex)
            {
                _onholdQueue.Clear();
            }
        }

        public void Subscribe(RedisClient client)
        {
            _client = client;
        }

        private void WorkCycle()
        {
            while (true)
            {
                lock (_mutex)
                {
                    while (_onholdQueue.Count == 0)
                    {
                        Monitor.Wait(_mutex, 5);
                    }
                    var tmp = _onholdQueue;
                    _onholdQueue = _activeQueue;
                    _activeQueue = tmp;
                }
                while (_activeQueue.Count > 0)
                {
                    var operation = _activeQueue.Dequeue();
                    ProcessOperation(operation);
                }
            }
        }
        
        private void ProcessOperation(IOperation operation)
        {
            lock (_connectionLock)
            {
                if (!_engine.IsConnected || _client == null)
                {
                    return;
                }

                try
                {
                    operation.Process( _client );
                    if (operation.OperationType == OperationType.Get)
                    {
                        _responseHandler.OnResponse(operation);
                    }
                }
                catch (Exception e)
                {
                    Log.Info("cache: operation error, connection will be reseted; operation = " + operation.OperationType + "; " + e.Message);
                    Bro.Log.Error(e);
                    _engine.Disconnect();
                }
            }
        }
        
        public void Set(string key, string value, int ttl)
        {
            if (!_engine.IsConnected)
            {
                return;
            }

            var operation = new Operation( key, value, ttl );
            
            lock (_mutex)
            {
                _onholdQueue.Enqueue(operation);
                Monitor.Pulse(_mutex);
            }
        }

        public void Get(string key, Action<string> callback) 
        {
            if (!_engine.IsConnected)
            {
                callback(null);
                return;
            }
            
            var operation = new Operation( key, callback );
            _responseHandler.RegisterRequest( operation );
            
            lock (_mutex)
            {
                _onholdQueue.Enqueue(operation);
                Monitor.Pulse(_mutex);
            }
        }
        
        public void Get(List<string> keys, Action<List<string>> callback)
        {
            if (!_engine.IsConnected)
            {
                callback(null);
                return;
            }
            
            var operation = new BatchOperation( keys, callback );
            _responseHandler.RegisterRequest( operation );
            
            lock (_mutex)
            {
                _onholdQueue.Enqueue(operation);
                Monitor.Pulse(_mutex);
            }
        }
        
        public void Transaction(string key, int ttl, int attempts, CacheServerDelegate.ValueJsonCheck checker, CacheServerDelegate.ValueJsonCreate creator, Action<bool> callback)
        {
            if (!_engine.IsConnected)
            {
                callback(false);
                return;
            }
            
            var operation = new TransactionOperation(key,ttl,3,checker,creator, callback);
            _responseHandler.RegisterRequest( operation );
            
            lock (_mutex)
            {
                _onholdQueue.Enqueue(operation);
                Monitor.Pulse(_mutex);
            }
        }
    }
}