using System.Collections.Generic;

namespace Bro
{
    public abstract class UnionConfigStorage<K, V> : IUnionConfigStorage, IConfigProvider<K,V>  where V : class
    {
        private readonly Dictionary<K, IConfigProvider<V>> _configProviders = new Dictionary<K, IConfigProvider<V>>();
        
        public V GetConfig(K key)
        {
            if (_configProviders.FastTryGetValue(key, out var configProvider))
            {
                return configProvider.GetConfig();
            }
            return null;
        }

        protected void AddConfigSubStorage(K key, SyncSingleConfigStorage<V> storage)
        {
            if (_configProviders.ContainsKey(key))
            {
                _configProviders[key] = storage;
            }
            else
            {
                _configProviders.Add(key, storage);
            }
        }

        public abstract void AddConfigSubStorage(ConfigDetails details, IConfigProvider configProvider);
    }
}