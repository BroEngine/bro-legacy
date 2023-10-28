using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Server.Context;
using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure 
{
    public class CumulativeProviderModule<T> : IServerContextModule where T : class
    { 
        private IServerContext _context;
        
        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;

        public void Save(string token, T data, Action<bool> callback)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None, JsonSettings.AutoSettings);
            var cumulativeSetRequest = new DocumentCumulativeSetRequest();
            cumulativeSetRequest.Data.Value = json;
            cumulativeSetRequest.Token.Value = token;
            BrokerEngine.Instance.SendRequest(cumulativeSetRequest, (response) =>
            {
                if (response == null)
                {
                    Alert("broker :: cumulative save timeouts");
                    _context.Scheduler.Schedule(() => callback?.Invoke(false));
                }
                else
                {
                    _context.Scheduler.Schedule(() => callback?.Invoke(((DocumentCumulativeSetResponse)response).Successfully.Value));
                }
            });
        }

        public void Load(string token, Action<List<T>> callback)
        {
            var cumulativeGetRequest = new DocumentCumulativeGetRequest();
            cumulativeGetRequest.Token.Value = token;
            BrokerEngine.Instance.SendRequest(cumulativeGetRequest, (response) =>
            {
                if (response != null)
                {
                    var list = new List<T>();
                    var getResponse = (DocumentCumulativeGetResponse) response;
                    foreach (var stringParam in getResponse.Data.Params)
                    {
                        var json = stringParam.Value;
                        try
                        {
                            var data = JsonConvert.DeserializeObject<T>(json);
                            list.Add(data);
                        }
                        catch (Exception) { /* ignored */ }
                    }
                    
                    _context.Scheduler.Schedule(() => callback?.Invoke(list));
                }
                else
                {
                    Alert("broker :: cumulative load timeouts");
                    _context.Scheduler.Schedule(() => callback?.Invoke(null));
                }
            });
        }

        public void Delete(string token)
        {
            var cumulativeDeleteRequest = new DocumentCumulativeDeleteRequest();
            cumulativeDeleteRequest.Token.Value = token;
            BrokerEngine.Instance.SendRequest(cumulativeDeleteRequest, (response) =>
            {
                if (response == null)
                {
                    Alert("broker :: cumulative delete timeouts");
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