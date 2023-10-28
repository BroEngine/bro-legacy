using System.Collections.Generic;
using Bro.Json;

namespace Bro
{
    public class CollectionModel<K, V>
    {
        [JsonProperty("default_config")] public K DefaultConfigId = default(K);
        [JsonProperty("data")] public Dictionary<K, V> Data = new Dictionary<K, V>();

        public V GetDefaultConfig()
        {
            V result;
            if (!Data.TryGetValue(DefaultConfigId, out result))
            {
                result = default(V);
            }

            return result;
        }
    }
}