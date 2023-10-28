using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Bro.Json;

namespace Bro
{
    public class SyncDictionaryConfigStorage<K, V> : IConfigProvider<K,V>, IConfigStorage where V : class
    {
        private FieldInfo _dictionaryKeyFieldInfo;
        private readonly object _sync = new object();
        private CollectionModel<K, V> _configCollection;
        private JsonSerializerSettings SerializerSettings => JsonSettings.AutoSettings;
        protected virtual Formatting DumpFormattingSetting => Formatting.Indented;
        bool IConfigStorage.IsInitialized => _configCollection != null;
        public int Version { get; private set; }
        protected virtual SurrogateSelector SurrogateSelector { get; set; }
        
        public IReadOnlyDictionary<K, V> Data
        {
            get
            {
                lock (_sync)
                {
                    return _configCollection.Data;
                }
            }
        }

        public SyncDictionaryConfigStorage()
        {
            lock (_sync)
            {
                _configCollection = new CollectionModel<K, V>();
            }
        }

        public virtual void Initialize(string configData, int version)
        {
            try
            {
                lock (_sync)
                {
                    Version = version;
                    
                    if (string.IsNullOrEmpty(configData))
                    {
                        _configCollection = new CollectionModel<K, V>();
                    }
                    else
                    {
                        _configCollection = JsonConvert.DeserializeObject<CollectionModel<K, V>>(configData, SerializerSettings);
                    }

                    OnConfigsInitialized(_configCollection);
                }
            }
            catch (Exception exp)
            {
                Bro.Log.Error("sync dictionary storage :: configs initialization error, json = " + configData);
                Bro.Log.Error(exp);
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

        public void ReinitConfigs(string configData, int version)
        {
            try
            {
                lock (_sync)
                {
                    var newConfigCollection = JsonConvert.DeserializeObject<CollectionModel<K, V>>(configData, SerializerSettings);

                    Version = version;
                    _configCollection.DefaultConfigId = newConfigCollection.DefaultConfigId;

                    var prevData = _configCollection.Data;
                    var newData = newConfigCollection.Data;
                    // remove old
                    foreach (var item in prevData)
                    {
                        if (!newData.ContainsKey(item.Key))
                        {
                            prevData.Remove(item.Key);
                        }
                    }

                    // add new 
                    foreach (var item in newData)
                    {
                        if (!prevData.ContainsKey(item.Key))
                        {
                            prevData.Add(item.Key, item.Value);
                        }
                    }

                    // update current
                    Type type = typeof(V);
                    var fieldsToUpdate = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var item in prevData)
                    {
                        var newItemValue = newData[item.Key];
                        ObjectCopier.CloneFields(item.Value, newItemValue, fieldsToUpdate);
                    }
                    OnConfigsInitialized(_configCollection);
                }
            }
            catch (Exception exp)
            {
                Bro.Log.Error(exp);
                Bro.Log.Info("json = " + configData);
            }
        }

        protected virtual void OnConfigsInitialized(CollectionModel<K, V> configCollection)
        {
            SetKeyInConfigs();
        }
        
        private void SetKeyInConfigs()
        {
            _dictionaryKeyFieldInfo = null;
            var props = typeof(V).GetFields();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                if (attrs.OfType<DictionaryKeyAttribute>().Any())
                {
                    _dictionaryKeyFieldInfo = prop;
                    break;
                }
            }

            if (_dictionaryKeyFieldInfo == null)
            {
                return;
            }
            
            foreach (var config in _configCollection.Data)
            {
                _dictionaryKeyFieldInfo.SetValue(config.Value, config.Key);
            }
        }

        public V GetDefaultConfig()
        {
            lock (_sync)
            {
                return _configCollection.GetDefaultConfig();
            }
        }

        public V GetConfig(K configId)
        {
            lock (_sync)
            {
                if (_configCollection.Data.TryGetValue(configId, out var result))
                {
                    return result;
                }
            }

            return null;
        }

        public V GetCopyOfConfig(K configId)
        {
            var cfg = GetConfig(configId);
            var result = ObjectCopier.Clone(cfg, SurrogateSelector);
            if (_dictionaryKeyFieldInfo != null && cfg != null)
            {
                _dictionaryKeyFieldInfo.SetValue(result, configId);
            }

            return result;
        }

        public void ForEachConfig(Action<V> action)
        {
            lock (_sync)
            {
                if (_configCollection != null)
                {
                    foreach (var pair in _configCollection.Data)
                    {
                        action(pair.Value);
                    }
                }
            }
        }

        public V GetConfig(Predicate<V> condition)
        {
            lock (_sync)
            {
                foreach (var pair in _configCollection.Data)
                {
                    if (condition(pair.Value))
                    {
                        return pair.Value;
                    }
                }
            }

            return null;
        }

        public void Clear()
        {
            lock (_sync)
            {
                _configCollection.Data.Clear();
            }
        }

        public void Replace(K key, V value)
        {
            lock (_sync)
            {
                _configCollection.Data[key] = value;
            }
        }

        public string Dump()
        {
            lock (_sync)
            {
                var result = String.Empty;
                try
                {
                    result = JsonConvert.SerializeObject(_configCollection, DumpFormattingSetting, SerializerSettings);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return result;
            }
        }
    }
}