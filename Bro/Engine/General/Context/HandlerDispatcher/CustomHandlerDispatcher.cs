using System;
using System.Collections.Generic;

namespace Bro
{
    public class CustomHandlerDispatcher : BaseHandlerDispatcher
    {
        public delegate void AttachHandler(Delegate dlg);

        public class HandlerInfo
        {
            public AttachHandler AttachHandler;
            public Type AttributeType; // typeof(AmmoCreatedHandlerAttribute)
            public Type HandlerType; // typeof(Action<float>)
        }
        
        private readonly Dictionary<Type, HandlerInfo> _handlers = new Dictionary<Type, HandlerInfo>();
        private readonly List<object> _attachedObjects = new List<object>();


        public void Clear()
        {
            _attachedObjects.Clear();
            _handlers.Clear();
        }

        public void AddHandlers(IList<HandlerInfo> handlers)
        {
            if (handlers != null)
            {
                foreach (var h in handlers)
                {
                    _handlers.Add(h.AttributeType, h);
                }
            }
        }
        
        public void AttachCurrentObjectToAllHandlers(object target)
        {
            AttachHandlersToObject(target, _handlers);
            _attachedObjects.Add(target);
        }

        private void AttachHandlersToObject(object target, Dictionary<Type, HandlerInfo> handlers)
        {
            var targetType = target.GetType();
            var objectMethodsData = GetObjectMethodsData(targetType);
            foreach (var hierarchyLevel in objectMethodsData.HierarchyData)
            {
                foreach (var handlerData in hierarchyLevel.Value)
                {
                    var cachedAttributeType = handlerData.AttributeType;
                    var cachedMethod = handlerData.Method;
                    HandlerInfo customHandlerInfo;
                    if (handlers.TryGetValue(cachedAttributeType, out customHandlerInfo ))
                    {
                        try
                        {
                            var dlg = Delegate.CreateDelegate(customHandlerInfo.HandlerType, target, cachedMethod);
                            customHandlerInfo.AttachHandler(dlg);
                        }
                        catch (Exception e)
                        {
                            Bro.Log.Error("Error init of customHandlerInfo  " + target.GetType() + " " + cachedMethod.Name + "\n exc = " + e);
                        }
                    }
                }
            }
        }

        public void ProcessCustomHandlers(object subscribedObject, IList<HandlerInfo> customHandlers)
        {            
            var handlerMap = new Dictionary<Type, HandlerInfo>();
            if (customHandlers != null && customHandlers.Count > 0)
            {
                foreach (var handler in customHandlers)
                {
                    handlerMap.Add(handler.AttributeType, handler);
                }
                AddHandlers(customHandlers);
            }

            foreach (var o in _attachedObjects)
            {
                AttachHandlersToObject(o, handlerMap);
            }
            
            AttachCurrentObjectToAllHandlers(subscribedObject);
        }   
    }
}