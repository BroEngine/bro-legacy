using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Server.Context;
using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class RegistryProviderModule : IServerContextModule
    {
        private IServerContext _context;
        private string _publicIp = string.Empty;
        private int _serverId = 0;
        private readonly ServerConfig _serverConfig;

        public int ServerId => _serverId;

        public RegistryProviderModule(ServerConfig serverConfig)
        {
            _serverConfig = serverConfig;
        }

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
            GetPublicIp();
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;

        [UpdateHandler(updatePeriod: 5000L)]
        private void Update()
        {
            Set();
            Get();
        }
        
        private void GetPublicIp()
        {
            var request = new Bro.Network.Web.WebRequest<MasterResponse>(_context, _serverConfig.MasterUri);
            request.Load((response) =>
            {
                if (response.Result)
                {
                    _publicIp = response.Data.Ip;
                    Bro.Log.Info("registry :: public ip set to " + _publicIp);
                }
                else
                {
                    Bro.Log.Info("registry :: public ip getting error");
                }
            });
        }

        private void SetAsEditor()
        {
            if (_serverId == 0)
            {
                _serverId = Random.Instance.Range(-100000, -100);
                Bro.Log.Info("registry :: server registered with server id = " + _serverId);
            }
        }

        private void Set()
        {
            #if UNITY_EDITOR
            SetAsEditor();
            return;
            #endif
            
            var version = AssemblyInfo.Version;
            var type = AssemblyInfo.ApplicationType;

            if (string.IsNullOrEmpty(type))
            {
                return;
            }

            if (string.IsNullOrEmpty(_publicIp))
            {
                GetPublicIp();
                return;
            }
            
            var server = new GameServer
            {
                Type = type,
                Ip = _publicIp,
                Port = _serverConfig.UdpPort, 
                Version = version
            };

            var loadRequest = new RegistrySetServerRequest();
            loadRequest.GameServer.Value = server;
            
            BrokerEngine.Instance.SendRequest(loadRequest, (response) =>
            {
                if (response != null)
                {
                    var loadResponse = (RegistrySetServerResponse) response;
                    var serverId = loadResponse.ServerId.Value;

                    if (_serverId == 0 && serverId > 0)
                    {
                        _serverId = serverId;
                        Bro.Log.Info("registry :: server registered with server id = " + serverId);
                    }
                }
                else
                {
                    Bro.Log.Info("registry :: server registration timeout");
                }
            });
            
            
        }

        private readonly List<GameServer> _servers = new List<GameServer>();
        private void Get()
        {
            var version = AssemblyInfo.Version;
            BrokerEngine.Instance.SendRequest(new RegistryGetServersRequest(), (response) =>
            {
                if (response != null)
                {
                    var getResponse = (RegistryGetServersResponse) response;
                    var serverParams = getResponse.Servers.Params;
                    _servers.Clear();
                    
                    foreach (var serverParam in serverParams)
                    {
                        var server = serverParam.Value;
                        if (server.Version == version)
                        {
                            _servers.Add(server);
                        }
                    }
                }
                else
                {
                    Bro.Log.Error("### test :: get servers failed");
                }
            });
        }

        // !!! ТУДУ ПРИВЕСТИ КЛАСС В ПОРЯДОК

        public GameServer GetRandomServer(string type)
        {
            var servers = new List<GameServer>();
            foreach (var server in _servers)
            {
                if (server.Type == type)
                {
                    servers.Add(server);
                }
            }

            if (servers.Count > 0)
            {
                return servers.Random();
            }

            return new GameServer();
        }



        [Serializable]
        private class MasterResponse
        {
            #pragma warning disable 0649
            [JsonProperty("ip")] public string Ip;
        }
    }
}