using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bro.Toolbox;

namespace Bro.Ecs
{
    public static class ComponentsRegistry
    {
        private static readonly Dictionary<byte, Type> SerializationTypes = new Dictionary<byte, Type> ();
        private static readonly Dictionary<Type, byte> SerializationCodes = new Dictionary<Type, byte> ();
        
        private static readonly Dictionary<byte, Type> RegularTypes = new Dictionary<byte, Type> ();
        private static readonly Dictionary<Type, byte> RegularCodes = new Dictionary<Type, byte> ();
        
        public static List<Type> GetSerializationTypes => SerializationCodes.Keys.ToList();
        public static List<Type> GetRegularTypes => RegularCodes.Keys.ToList();

         
        public static void Initialize() 
        {
            // its call static constructor, if it not called before
        }
        
       
        static ComponentsRegistry()
        {
            var serializationData = new Dictionary<string, Type>();
            var regularData = new Dictionary<string, Type>();
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
          
            foreach (var assembly in assemblies)
            {
                #if CONSOLE_CLIENT
                if (assembly.FullName.Contains("Unity"))
                {
                    continue;
                }
                #endif
                
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(SerializationComponentAttribute), true).Length > 0)
                    {
                        var name = type.FullName;
                        if (name != null)
                        {
                            serializationData.Add(name, type);
                        }
                    }
                    
                    if (type.GetCustomAttributes(typeof(ComponentAttribute), true).Length > 0)
                    {
                        var name = type.FullName;
                        if (name != null)
                        {
                            regularData.Add(name, type);
                        }
                    }
                }
            }

           
            var serializationSorted = serializationData.OrderBy(x => x.Key);
            var regularSorted = regularData.OrderBy(x => x.Key);
            
            var index = 0;
            
            foreach (var sortedPair in serializationSorted)
            {
                var type = sortedPair.Value;
                RegisterSerialization((byte) ++index, type);
            }
            
            foreach (var sortedPair in regularSorted)
            {
                var type = sortedPair.Value;
                RegisterRegular((byte) ++index, type);
            }
        }
        
        private static void RegisterSerialization(byte code, Type type)
        {
            SerializationTypes.Add(code, type);
            SerializationCodes.Add(type, code);
        }
        
        private static void RegisterRegular(byte code, Type type)
        {
            RegularTypes.Add(code, type);
            RegularCodes.Add(type, code);
        }

        public static byte GetTypeCode(Type type)
        {
            #if UNITY_EDITOR
            if (!SerializationCodes.ContainsKey(type))
            {
                Bro.Log.Error("ecs: not registered component type = " + type);
            }
            #endif
            return SerializationCodes[type]; 
        }
        
        public static bool IsRegistered(Type type)
        {
            return SerializationCodes.ContainsKey(type);
        } 
        
        public static Type GetType(byte typeCode)
        {
            #if UNITY_EDITOR
            if (!SerializationTypes.ContainsKey(typeCode))
            {
                Bro.Log.Error("type code = " + typeCode + " not registered");
                foreach (var typePair in SerializationTypes)
                {
                    Bro.Log.Error("--- registered type = " + typePair.Value + ", code = " + typePair.Key);
                }
            }
            #endif
            return SerializationTypes[typeCode];
        }

    }
}