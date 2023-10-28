using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public static class ParamsRegistry
    {
        public delegate BaseParam CreateParamFunc();
        
        public class ParamData
        {
            public CreateParamFunc CreateParam;
            public byte ParamTypeIndex;
        }
        
        private static readonly Dictionary<Type, ParamData> _registeredParamTypes = new Dictionary<Type, ParamData>();
        private static readonly List<byte> _paramTypeIndexList = new List<byte>();
        
         public static void Register(Type type, CreateParamFunc createParam, byte paramTypeIndex)
        {
            #if UNITY_EDITOR
            var param = createParam();
            if (!(param is IObjectParam))
            {
                Bro.Log.Error($"universal param :: {param.GetType()} should support IObjectParam to use it in UniversalParam");
            }

            if (_registeredParamTypes.ContainsKey(type))
            {
                Bro.Log.Error($"universal param :: this type {type} is already registered");
                return;
            }
            #endif
            
            if (_paramTypeIndexList.Contains(paramTypeIndex))
            {
                Bro.Log.Error($"universal param :: parameter with index{paramTypeIndex} is already registered");
                return;
            }

            var sampleParam = createParam.Invoke();
            if (!(sampleParam is IObjectParam))
            {
                Bro.Log.Error("universal param :: param should implement IObjectParam");
                return;
            }
            if ((sampleParam as IObjectParam).ValueType != type)
            {
                Bro.Log.Error($"universal param :: invalid registering value type {(sampleParam as IObjectParam).ValueType} != {type}");
                return;
            }

            _paramTypeIndexList.Add(paramTypeIndex);
            _registeredParamTypes.Add(type, new ParamData()
            {
                CreateParam = createParam,
                ParamTypeIndex = paramTypeIndex
            });
        }

        public static bool IsRegistered(Type type)
        {
            return _registeredParamTypes.ContainsKey(type);
        }

        public static ParamData GetParamData(Type type)
        {
            if (_registeredParamTypes.ContainsKey(type))
            {
                return _registeredParamTypes[type];
            }
            else
            {
                Bro.Log.Error("no param for type = " + type);
            }
            return null;
        }

        public static ParamData GetParamData(byte paramTypeIndex)
        {
            foreach (var data in _registeredParamTypes)
            {
                if (data.Value.ParamTypeIndex == paramTypeIndex)
                {
                    return data.Value;
                }
            }

            return null;
        }
    }
}