using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Server.Context;
using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class TemporaryProviderModule<T> : IServerContextModule where T : class
    {
        private IServerContext _context;
        
        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;
        
        public void Save(T data, Action<string> callback)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None, JsonSettings.AutoSettings);
            
            var temporarySetRequest = new DocumentTemporarySetRequest();
            temporarySetRequest.Data.Value = json;
            
            BrokerEngine.Instance.SendRequest(temporarySetRequest, (response) =>
            {
                if (response != null)
                {
                    var setResponse = (DocumentTemporarySetResponse) response;
                    var token = setResponse.Token.Value;
                    _context.Scheduler.Schedule(() => callback?.Invoke(token));
                }
                else
                {
                    Alert("temporary storage :: temporary save timeouts");
                    _context.Scheduler.Schedule(() => callback?.Invoke(null));
                }
            });
        }

        public void Load(string token, Action<T> callback)
        {
            var temporaryGetRequest = new DocumentTemporaryGetRequest();
            temporaryGetRequest.Token.Value = token;
            BrokerEngine.Instance.SendRequest(temporaryGetRequest, (response) =>
            {
                if (response != null)
                {
                    var setResponse = (DocumentTemporaryGetResponse) response;
                    var json = setResponse.Data.Value;
                    
                    try
                    {
                        var data = JsonConvert.DeserializeObject<T>(json, JsonSettings.AutoSettings);
                        _context.Scheduler.Schedule(() => callback?.Invoke(data));
                    }
                    catch (Exception)
                    {
                        Alert("broker :: temporary deserialization failed");
                        _context.Scheduler.Schedule(() => callback?.Invoke(null));
                    }
                }
                else
                {
                    Alert("broker :: temporary load timeouts");
                    _context.Scheduler.Schedule(() => callback?.Invoke(null));
                }
            });
        }

        private void Alert(string message)
        {
            Bro.Log.Error(message);
            #warning todo invoke global alert system
        }
    }
}