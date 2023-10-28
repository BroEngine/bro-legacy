using System;
using Bro.Client;
using Bro.Json;
using Bro.Network;

namespace Bro.Sketch.Client
{
    public class LoadGeoDataTask : SubscribableTask<LoadGeoDataTask>
    {
        private const string Gate = "http://www.geoplugin.net/json.gp"; /* do not replace in config */

        // ReSharper disable InconsistentNaming
        public string ContinentISOCode { get; private set; }
        public string CountryISOCode { get; private set; }
        private readonly IClientContext _context;
        
        public LoadGeoDataTask(IClientContext context)
        {
            _context = context;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            Load();
        }

        private void Load()
        {
            var request = new Bro.Network.Web.RegularWebRequestTask( _context, Bro.Network.Web.RegularWebRequestTask.RequestType.Get, Gate);
            request.OnComplete += (task => { OnResponse(task.Response); });
            request.OnFail += (task => { OnResponse(null); });
            request.Launch(_context);
        }
        
        private void OnResponse(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<Model>(response);
                    if (model != null)
                    {
                        ContinentISOCode = model.ContinentCode;
                        CountryISOCode = model.CountryCode;
                        
                        Bro.Log.Info("Loaded geo data, continent = " + ContinentISOCode + ", country " + CountryISOCode);
                        
                        Complete();
                        return;
                    }
                }
                catch (Exception)
                {
                    Bro.Log.Error("geo json parse error, json = " + response);
                }
            }
            Fail();
        }

        private class Model
        {
            [JsonProperty("geoplugin_countryCode")] public string CountryCode;
            [JsonProperty("geoplugin_continentCode")] public string ContinentCode;
        }
    }
}