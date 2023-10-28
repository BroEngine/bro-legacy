using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Service.Cache;
using Bro.Threading;
using Sider;

namespace Bro.Service
{
    public class CacheServerEngine : Bro.StaticSingleton<ICacheServer,CacheServerEngine>, ICacheServer 
    {
        private const int ThreadMandatorySleep = 2000;
        
        public bool IsStarted { get; private set; }
        public bool IsConnected { get; private set; }

        private readonly object _connectionLock = new object();
        
        private CacheServerConfig _config;
        private OperationHandler _operator;
        private BroThread _thread;

        private RedisClient _client;
        
        void ICacheServer.Start(CacheServerConfig cacheServerConfig)
        {
            if (IsStarted)
            {
                Bro.Log.Info("cache :: already started, ignoring");
                return;
            }

            _config = cacheServerConfig;

            _operator = new OperationHandler(this, _connectionLock);
            _operator.Start();
            
            _thread = new BroThread(WorkCycle);
            _thread.Start();

            IsStarted = true;
        }

        public void Disconnect()
        {
            Bro.Log.Info("cache :: connection resetting");
            IsConnected = false;
        }

        private void Reset()
        {
            lock (_connectionLock)
            {
                _operator.Reset();

                if (_client != null)
                {
                    _client.Reset();
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        private void Connect()    
        {
            lock (_connectionLock)
            {
                Bro.Log.Info("cache :: start connection" );
                
                var settings = RedisSettings.Build().Host(_config.ConfigurationGate).Port(_config.Port);
                _client = new RedisClient(settings);
                _operator.Subscribe(_client);

                IsConnected = true;
                
                Bro.Log.Info("cache :: connected to " + _config.ConfigurationGate );
            }
        }

        private void WorkCycle()
        {
            while (true)
            {
                if ( ! IsConnected )
                {
                    try
                    {
                        Reset();
                        Connect();
                    }
                    catch (Exception e)
                    {
                        Bro.Log.Error("cache :: connection failed " + e.Message);
                        Bro.Log.Error(e);
                    }                    
                }
            
                Thread.Sleep(ThreadMandatorySleep);
            }
        }

        void ICacheServer.Set(string key, string value, int ttl)
        {
            _operator.Set(key,value, ttl);
        }

        void ICacheServer.Get(string key, Action<string> callback) 
        {
            _operator.Get(key,callback);
        }
        
        void ICacheServer.Get(List<string> keys, Action<List<string>> callback)
        {
            _operator.Get(keys,callback);
        }
        
        void ICacheServer.Transaction(string key, int ttl, int attempts, CacheServerDelegate.ValueJsonCheck checker, CacheServerDelegate.ValueJsonCreate creator, Action<bool> callback)
        {
            _operator.Transaction(key,ttl,attempts, checker, creator, callback);
        }
        
        void ICacheServer.Transaction(string key, string value, int ttl, int attempts, Action<bool> callback)
        {
            _operator.Transaction(key,ttl,attempts, null, (v) => { return value; }, callback);
        }
    }
}