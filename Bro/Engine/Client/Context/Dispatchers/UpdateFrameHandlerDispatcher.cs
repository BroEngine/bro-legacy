using System;
using System.Reflection;


namespace Bro.Client
{
    public class UpdateFrameHandlerDispatcher : BaseHandlerDispatcher
    {
        public void AttachHandlersToObject(object target, Timing.IScheduler scheduler)
        {
            var targetType = target.GetType();
            var objectMethodsData = GetObjectMethodsData(targetType);
            foreach (var hierarchyLevel in objectMethodsData.HierarchyData)
            {
                foreach (var handlerData in hierarchyLevel.Value)
                {
                    var cachedAttributeType = handlerData.AttributeType;
                    var cachedMethod = handlerData.Method;
                    var cachedAttribute = handlerData.Attribute;
                    if (cachedAttributeType == typeof(UpdateHandlerAttribute))
                    {
                        Bro.Log.Error("error :: do not use UpdateHandler on the client. use UpdateFrameHandler instead");
                    } 
                    if (cachedAttributeType == typeof(UpdateFrameHandlerAttribute))
                    {
                        AttachUpdateHandler(target, cachedMethod, cachedAttribute, scheduler);
                    }
                }
            }
        }
        
        private static void AttachUpdateHandler(object target, MethodInfo method, object attribute, Timing.IScheduler scheduler)
        {
            if (method.GetParameters().Length == 1)
            {
                var action = Delegate.CreateDelegate(typeof(Timing.Handler), target, method) as Timing.Handler;
                scheduler.ScheduleUpdate(action);
            }
            else
            {
                Bro.Log.Error($"update function should have one float type parameter  {target.GetType()} {method.Name}");
            }
        }
    }
}