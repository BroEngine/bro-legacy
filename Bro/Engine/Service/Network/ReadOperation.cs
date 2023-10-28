using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Service.Context;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bro.Service
{
    public class ReadOperation
    {
        private readonly BrokerEngine _engine;
        private readonly BrokerConfig _config;

        private readonly List<IServiceDispatcher> _dispatchers = new List<IServiceDispatcher>();
        private readonly ResponseOperation _response;
        
        public ReadOperation( BrokerEngine engine, BrokerConfig config, ResponseOperation response, object connectionLock, object operationLock )
        {
            _engine = engine;
            _config = config;
            _response = response;
        }

        public void Start()  { }
      
        public void Reset() { }
        
        private void OnReceive(IServiceRequest serviceRequest)
        {
            foreach (var handler in _dispatchers)
            {
                handler.HandleServiceRequest( serviceRequest );
            }
        }
        
        private void OnReceive(IServiceEvent serviceEvent)
        {
            foreach (var handler in _dispatchers)
            {
                handler.HandleServiceEvent( serviceEvent );
            }
        }
        
        private void OnReceive(IServiceResponse serviceResponse)
        {
            _response.OnResponse( serviceResponse );
        }

        private void Receive(IServiceOperation operation)
        {
            var point = PerformanceMeter.Register(PerformancePointType.BrokenReadHandler);
            
            if (operation != null)
            {
                switch (operation.Type)
                {
                    case ServiceOperationType.Request:
                        OnReceive(operation as IServiceRequest);
                        break;
                    case ServiceOperationType.Response:
                        OnReceive(operation as IServiceResponse);
                        break;
                    case ServiceOperationType.Event:
                        OnReceive(operation as IServiceEvent);
                        break;
                }
            }
            
            point?.Done();
        }
        
        private void OnMessage(object o, BasicDeliverEventArgs args)
        {
            if (o == null || args == null)
            {
                Bro.Log.Error("broker :: received empty message");
                return;
            }
            
            var point = PerformanceMeter.Register(PerformancePointType.BrokenReadFull);
            
            var data = args.Body;
            
            using (var reader = DataReader.GetBinaryReader(data))
            {
                while (!reader.IsEndOfData)
                {
                    var netOperation = ServiceOperationFactory.Deserialize(reader);
                    Receive(netOperation);
                }
            }
            
            point?.Done();
        }

        public void RegisterDispatcher(IServiceDispatcher dispatcher)
        {
            if (!_dispatchers.Contains(dispatcher))
            {
                _dispatchers.Add(dispatcher);
            }
        }

        public void RemoveDispatcher(IServiceDispatcher dispatcher)
        {
            _dispatchers.Remove(dispatcher);
        }

        public void Subscribe(IModel session)
        {
            foreach (var channel in _config.Channels)
            {
                var consumer = new EventingBasicConsumer(session);
                consumer.Received += OnMessage;
                session.QueueDeclare( queue: channel.Path, durable: false, exclusive: false, autoDelete: true, arguments: null);
                session.BasicConsume( queue: channel.Path, noAck: true, consumer: consumer );
            }
        }
    }
}