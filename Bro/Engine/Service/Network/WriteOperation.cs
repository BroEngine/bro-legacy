using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Network;
using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Threading;
using RabbitMQ.Client;

namespace Bro.Service
{
    public class WriteOperation
    {
        private BroThread _thread;
        
        private Queue<IServiceOperation> _activeQueue = new Queue<IServiceOperation>();
        private Queue<IServiceOperation> _onholdQueue = new Queue<IServiceOperation>();
        
        private readonly object _mutex = new object();
        private readonly BrokerEngine _engine;
        private readonly BrokerConfig _config;
        private readonly IWriter _dataWriter = DataWriter.GetBinaryWriter(NetworkConfig.MessageMaxSize);
        private readonly object _connectionLock;
        private readonly object _operationLock;
        private readonly ResponseOperation _response;

        private readonly List<string> _declarations = new List<string>();
        
        private IModel _session;
        
        public WriteOperation( BrokerEngine engine, BrokerConfig config, ResponseOperation response, object connectionLock, object operationLock )
        {
            _engine = engine;
            _config = config;
            _response = response;
            _connectionLock = connectionLock;
            _operationLock = operationLock;
        }

        public void Start() 
        {
            _thread = new BroThread(WorkCycle);
            _thread.Start();
        }
        
        public void Reset()
        {
            _declarations.Clear();

            lock (_mutex)
            {
                _onholdQueue.Clear();
            }
        }
        
        public void Subscribe(IModel session)
        {
            _session = session;
        }

        public void Send(IServiceOperation operation)
        {
            if (!_engine.IsConnected)
            {
                return;
            }

            lock (_mutex)
            {
                _onholdQueue.Enqueue(operation);
                Monitor.Pulse(_mutex);
            }
        }

        public void SendRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback)
        {
            if (!_engine.IsConnected)
            {
                callback?.Invoke(null);
                return;
            }
            
            _response.RegisterRequest( serviceRequest, callback );
           
            lock (_mutex)
            {
                _onholdQueue.Enqueue(serviceRequest);
                Monitor.Pulse(_mutex);
            }
        }
        
        private void ProcessSend(IServiceOperation operation)
        {
            lock (_connectionLock)
            {
                if (!_engine.IsConnected)
                {
                    return;
                }

                var point = PerformanceMeter.Register(PerformancePointType.BrokenWrite);
                
                try
                {
                    var channel = operation.Channel;
                    var path = channel.Path;
                    
                    if (channel.PathType == ServicePathType.Composite)
                    {
                        path = _config.Path + path;
                    }
                    
                    Declare(path);
                    
                    ServiceOperationFactory.Serialize(_dataWriter, operation);
                    var data = _dataWriter.Data;
                    
                    var properties = _session.CreateBasicProperties();
                    properties.Persistent = false;
                    properties.Expiration = operation.ExpirationTimestamp.ToString();
                    _session.BasicPublish(exchange: string.Empty, routingKey: path, basicProperties: properties, body: data);
                 
                    _dataWriter.Reset();
                }
                catch (Exception e)
                {
                    Log.Info("broker :: write error, connection will be reset; " + e.Message);
                    _engine.Disconnect();
                }
                
                point?.Done();
            }
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
                    ProcessSend(operation);
                }
            }
        }
      
        private void Declare(string path)
        {
            lock (_operationLock)
            {
                if (!_declarations.Contains(path))
                {
                    _session.QueueDeclare(queue: path, durable: false, exclusive: false, autoDelete: true, arguments: null);
                    _declarations.Add( path );
                }
            }
        }
    }
}