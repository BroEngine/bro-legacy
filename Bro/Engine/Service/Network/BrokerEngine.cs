using System;
using Bro.Network.Service;
using System.Threading;
using Bro.Service.Context;
using Bro.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bro.Service
{
    public class BrokerEngine : Bro.StaticSingleton<IBroker, BrokerEngine>, IBroker
    {
        static BrokerEngine()
        {
            AppDomain.CurrentDomain.ProcessExit += OnApplicationExit;
        }

        private const int ThreadMandatorySleep = 2000;

        private readonly object _connectionLock = new object();
        private readonly object _operationLock = new object();
        private BrokerConfig _config;
        private BroThread _thread;

        private IModel _session;
        private IConnection _connection;

        private ReadOperation _read;
        private WriteOperation _write;
        private ResponseOperation _response;

        public bool IsConnected { get; private set; }
        public bool IsStarted { get; private set; }

        public IServiceChannel Channel => _config.PrivateChannel;

        void IBroker.Start(BrokerConfig brokerConfig)
        {
            
            if (IsStarted)
            {
                Bro.Log.Info("broker :: already started, ignoring");
                return;
            }

            lock (_operationLock)
            {
                _config = brokerConfig;

                _response = new ResponseOperation(_config.PrivateChannel);
                _read = new ReadOperation(this, _config, _response, _connectionLock, _operationLock);
                _write = new WriteOperation(this, _config, _response, _connectionLock, _operationLock);

                _thread = new BroThread(WorkCycle);
                _thread.Start();

                _read.Start();
                _write.Start();
                _response.Start();

                IsStarted = true;
            }
        }

        IServiceChannel IBroker.PrivateChannel
        {
            get
            {
                if (_config != null && _config.PrivateChannel != null)
                {
                    return _config?.PrivateChannel;
                }
                return new PrivateChannel();
            }
        }

        public void Disconnect()
        {
            Bro.Log.Info("broker :: disconnected");
            IsConnected = false;
        }

        private void Reset()
        {
            Bro.Log.Info("broker :: resetting connection");
            lock (_connectionLock)
            {
                _read.Reset();
                _write.Reset();
                _response.Reset();

                if (_connection != null)
                {
                    _connection.CallbackException -= ConnectionExceptionListener;
                    _connection.ConnectionShutdown -= ConnectionShutdownListener;
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }

                if (_session != null)
                {
                    _session.Close();
                    _session.Dispose();
                    _session = null;
                }
            }
        }

        private void WorkCycle()
        {
            while (true)
            {
                if (!IsConnected)
                {
                    try
                    {
                        Reset();
                    }
                    catch (Exception e)
                    {
                        Bro.Log.Error("broker :: reset connection failed " + e.Message);
                        Bro.Log.Error(e);
                    }

                    try
                    {
                        Connect();
                    }
                    catch (Exception e)
                    {
                        Bro.Log.Error("broker :: connection failed " + e.Message);
                        Bro.Log.Error(e);
                    }
                }

                Thread.Sleep(ThreadMandatorySleep);
            }
        }

        private void Connect()
        {
            lock (_connectionLock)
            {
                Bro.Log.Info("broker :: start connection " + _config.Host + ":" + _config.Port);

                var factory = new ConnectionFactory()
                {
                    HostName = _config.Host,
                    Port = _config.Port,
                    UserName = _config.User,
                    Password = _config.Password,
                    SocketReadTimeout = 10000,
                    SocketWriteTimeout = 10000,
                    RequestedConnectionTimeout = 10000,
                };

                _connection = factory.CreateConnection();
                _session = _connection.CreateModel();

                _connection.CallbackException += ConnectionExceptionListener;
                _connection.ConnectionShutdown += ConnectionShutdownListener;

                _read.Subscribe(_session);
                _write.Subscribe(_session);

                IsConnected = true;

                Bro.Log.Info("broker :: connected to " + factory.Endpoint.HostName);
            }
        }

        private void ConnectionShutdownListener(object o, ShutdownEventArgs args)
        {
            Bro.Log.Info("broker :: connection interrupted");
            Disconnect();
        }

        private void ConnectionExceptionListener(object o, CallbackExceptionEventArgs args)
        {
            Bro.Log.Info("broker :: Exception received, connection will be reset; " + args.Exception?.Message);
            Disconnect();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            if (_instance != null)
            {
                _instance.Disconnect();
                Bro.Log.Info("broker :: gracefully exited");
            }
        }

        void IBroker.Send(IServiceOperation operation)
        {
            lock (_connectionLock)
            {
                _write.Send(operation);
            }
        }

        void IBroker.SendRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback)
        {
            lock (_connectionLock)
            {
                _write.SendRequest(serviceRequest, callback);
            }
        }
        
        void IBroker.SendRequest(IServiceRequest serviceRequest)
        {
            lock (_connectionLock)
            {
                _write.SendRequest(serviceRequest, null);
            }
        }

        void IBroker.RegisterDispatcher(IServiceDispatcher dispatcher)
        {
            lock (_operationLock)
            {
                if (_read != null)
                {
                    _read.RegisterDispatcher(dispatcher);
                }
            }
        }

        void IBroker.RemoveDispatcher(IServiceDispatcher dispatcher)
        {
            lock (_operationLock)
            {
                if (_read != null)
                {
                    _read.RemoveDispatcher(dispatcher);
                }
            }
        }
    }
}