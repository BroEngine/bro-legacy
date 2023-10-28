using System;
using Bro.Json;

namespace Bro
{
    public class SyncSingleConfigStorage<T> : IConfigProvider<T>, Bro.IConfigStorage where T : class
    {
        protected SingleModel<T> _configModel;
        private readonly object _sync = new object();
        protected virtual Formatting DumpFormattingSetting => Formatting.Indented;
        protected virtual JsonSerializerSettings SerializerSettings => JsonSettings.AutoSettings;
        
        public int Version { get; private set; }
        public bool IsInitialized => _configModel != null;
        
        public T GetConfig()
        {
            return _configModel?.Data;
        }
        
        public virtual void Initialize(string configData, int version)
        {
            if (string.IsNullOrEmpty(configData))
            {
                Bro.Log.Error($"sync single config storage {GetType()} :: invalid config data => empty");
                return;
            }

            try
            {
                lock (_sync)
                {
                    Version = version;
                    _configModel = JsonConvert.DeserializeObject<SingleModel<T>>(configData, SerializerSettings);
                }
            }
            catch (Exception exp)
            {
                Bro.Log.Error($"sync single config storage :: error during parsing configs {typeof(T)}\n exception {exp}\n json = {configData}");
            }
        }

      
        public void CopyFrom(IConfigStorage source)
        {
            lock (_sync)
            {
                var configData = source.Dump();
                Initialize(configData, source.Version);
            }
        }

        public void Replace(T value)
        {
            if (_configModel == null)
            {
                _configModel = new SingleModel<T>();
            }
            _configModel.Data = value;
        }
        
        public string Dump()
        {
            lock (_sync)
            {
                return JsonConvert.SerializeObject(_configModel, DumpFormattingSetting, SerializerSettings);
            }
        }

        public void Reset()
        {
            lock (_sync)
            {
                _configModel = null;
            }
        }
    }
}