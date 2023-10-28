using System.Collections.Generic;
using Bro.Json;

namespace Bro
{
    public static class JsonConverters
    {
        private static readonly List<JsonConverter> _converters = new List<JsonConverter>();

        static JsonConverters()
        {
            AddConverter(new DataTableJsonConverter());
        }
        
        public static void AddConverter(JsonConverter converter)
        {
            if (!_converters.Contains(converter))
            {
                _converters.Add(converter);
            }
        }

        public static JsonSerializerSettings Settings
        {
            get
            {
                var result = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };

                for (var i = 0; i < _converters.Count; ++i)
                {
                    result.Converters.Add(_converters[i]);
                }

                return result;
            }
        }
    }
}