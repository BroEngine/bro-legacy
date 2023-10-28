using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Json.Linq;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class SharedController
    {
        private readonly List<EcsSystems> _systems = new List<EcsSystems>();
     

        public void AddSystems(EcsSystems system)
        {
            _systems.Add(system);
        }

        public void SetShared<T>(object o)
        {
            foreach (var system in _systems)
            {
                system.SetShared(typeof(T), o);   
            }
        }

        public string GetSharedAsJson()
        {
            Bro.Log.Assert(_systems.Count > 0);
            var shared = _systems[0].GetShared();
            
            var dictionary = new Dictionary<string, string>();
            foreach (var pair in shared)
            {
                var type = pair.Key;
                var typeString = type.FullName;
                if (typeString != null)
                {
                    dictionary.Add(typeString, JsonConvert.SerializeObject(pair.Value, Formatting.None, JsonSettings.AutoSettings));
                }
            }

            var result = JsonConvert.SerializeObject(dictionary);
            return result;
        }

        public Dictionary<Type, object> SetSharedFromJson(string json)
        {
            foreach (var system in _systems)
            {
                system.ClearShared();
            }
           
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var sharedData = new Dictionary<Type, object>();
            foreach (var dataPair in data)
            {
                var typeString = dataPair.Key;
                var type = Type.GetType(typeString);
                
                var o = JsonConvert.DeserializeObject(dataPair.Value, type, JsonSettings.AutoSettings); 

                foreach (var system in _systems)
                {
                    system.SetShared(type, o);
                }

                if (type != null)
                {
                    sharedData[type] = o;
                }
            }

            return sharedData;
        }
    }
}