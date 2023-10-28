using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public static class ClassTypeBinder
    {
        private static readonly Dictionary<string, Type> _registeredParamKeys = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, string> _registeredParamTypes = new Dictionary<Type, string>();
        
        public static void Register(Type type, string key)
        {
#if UNITY_EDITOR
            if (_registeredParamKeys.ContainsKey(key))
            {
                Bro.Log.Error($"class type vault :: type with key {key} is already registered");
                return;
            }
#endif

            _registeredParamKeys.Add(key, type);
            _registeredParamTypes.Add(type, key);
        }

        public static Type GetParamData(string key)
        {
            return _registeredParamKeys.FastTryGetValue(key, out var result) ? result : null;
        }

        public static string GetParamData(Type type)
        {
            return _registeredParamTypes.FastTryGetValue(type, out var result) ? result : null;
        }
    }
}