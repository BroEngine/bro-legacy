using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using Bro.Network;
using Bro.Network.Service;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class ServerInstance
    {
        private IServerCore _serverCore = null;
        private readonly ServerConfig _serverConfig;
        private readonly ConfigStorageCollector _configStorageCollector = new ConfigStorageCollector();

        public ServerInstance(ServerConfig config)
        {
            _serverConfig = config;
        }

        public void Start(BaseLoadConfigTask serverLoadConfigsTask, CommonContextStorage contextStorage, ITaskContext taskContext)
        {
            StartBroker();
            serverLoadConfigsTask.Collector = _configStorageCollector;
            serverLoadConfigsTask.OnComplete += (t=>
            {
                Bro.Log.Info("server instance :: complete load config task");
                contextStorage.ConfigStorageCollector = _configStorageCollector;
                ProcessStart(contextStorage);
            });
            serverLoadConfigsTask.OnFail += (t=>Bro.Log.Info("server instance :: fail load config task"));
            serverLoadConfigsTask.Launch(taskContext);
        }

        private void ProcessStart(ContextStorage contextStorage)
        {
            if (_serverConfig.IsOffline)
            {
                #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
                _serverCore = new OfflineCore();
                _serverCore.Start(_serverConfig.GetNetworkConfig(),contextStorage);
                #else
                Bro.Log.Error("offline server is not supported");
                #endif
            }
            else
            {
                StartOnline(contextStorage);
            }
        }

        

        private void StartOnline(ContextStorage contextStorage)
        {
            Bro.Log.SetLogFile($"logs/{_serverConfig.LogPrefix}.txt", 3);
            Bro.Log.UseSeparatedErrorFile = true;
            Bro.Log.UsePerformanceFile = true;
            Bro.Log.Settings.IsConsoleWriterEnabled = true;

            //Bro.Threading.ThreadPool.DebugMode = true;

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            _serverCore = new OnlineCore();
            _serverCore.Start(_serverConfig.GetNetworkConfig(), contextStorage);
        }

        public void Stop()
        {
            Bro.Log.Info("application :: stopping server application");

            if (_serverCore != null)
            {
                _serverCore.Stop();
                _serverCore = null;

                AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
                AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            }

            Bro.Threading.ThreadScheduler.TerminateAll();
            Bro.Threading.ThreadManagement.TerminateAll();

            Thread.Sleep(200);

            Bro.Log.Info("application :: server app is stopped");
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Bro.Log.Info("application :: on process exit event received");
            Stop();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            Bro.Log.Info("application :: unhandled exception, runtime will terminate = " + args.IsTerminating);
            Bro.Log.Error(e);
        }

        private void StartBroker()
        {
            if (!string.IsNullOrEmpty(_serverConfig.BrokerUri))
            {
                try
                {
                    var uri = new Uri(_serverConfig.BrokerUri);

                    var host = uri.Host;
                    var port = uri.Port;
                    var user = uri.UserInfo.Split(':')[0];
                    var pass = uri.UserInfo.Split(':')[1];
                    var path = uri.LocalPath.Replace("/", string.Empty);
                    
                    var config = new BrokerConfig( host, port, user, pass, path );            
                    BrokerEngine.Instance.Start( config );

                    #warning todo make it more elegant
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    
                    #if ! UNITY_EDITOR
                    while (!BrokerEngine.Instance.IsConnected || stopwatch.ElapsedMilliseconds < 1000L)
                    {   
                        
                        Thread.Sleep(33);
                        
                    }
                    #endif
                }
                catch(Exception)
                {
                    Bro.Log.Error("broker :: broker uri format invalid");
                }
            }
            else
            {
                Bro.Log.Error("broker :: broker uri not defined infrastructure will be unavailable");
            }
        }
    }
}