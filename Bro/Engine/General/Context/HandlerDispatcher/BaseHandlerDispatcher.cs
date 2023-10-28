using System.Collections.Generic;
using System.Reflection;
using System;

namespace Bro
{
    public class BaseHandlerDispatcher
    {
        private static readonly object _sync = new object();
        private static readonly Dictionary<Type, ObjectMethodsData> _handlerParamsCache = new Dictionary<Type, ObjectMethodsData>();
        
        public class ObjectMethodsData
        {
            public Dictionary<Type, List<MethodData>> HierarchyData;
        } 
        public class MethodData
        {
            public MethodInfo Method;
            public Type AttributeType;
            public object Attribute;
        }
        
        private ObjectMethodsData GetCache(Type type)
        {
            lock (_sync)
            {
                ObjectMethodsData result;
                if (_handlerParamsCache.TryGetValue(type, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }

        }
        
        public void UpdateCache(Type targetType, ObjectMethodsData handlerParams)
        {
            lock (_sync)
            {
                if (!_handlerParamsCache.ContainsKey(targetType))
                {
                    _handlerParamsCache[targetType] = handlerParams;
                }
            }
        }
        
        public ObjectMethodsData GetObjectMethodsData(Type targetType)
        {
            var objectMethodsData = GetCache(targetType);
            if (objectMethodsData == null)
            {
                objectMethodsData = new ObjectMethodsData()
                {
                    HierarchyData = new Dictionary<Type, List<MethodData>>()
                };
                var hierarchy = targetType.GetHierarchy();
                foreach (var type in hierarchy)
                {
                    objectMethodsData.HierarchyData[type] = new List<MethodData>();
                    var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                    foreach (var method in type.GetMethods(bindingFlags))
                    {
                        foreach (var attribute in method.GetCustomAttributes(true))
                        {
                            var attributeType = attribute.GetType();
                            objectMethodsData.HierarchyData[type].Add(new MethodData()
                            {
                                Method = method,
                                AttributeType = attributeType,
                                Attribute = attribute
                            });
                        }
                    }
                }
                UpdateCache(targetType, objectMethodsData);
            }

            return objectMethodsData;


        }
                

                
    }
}