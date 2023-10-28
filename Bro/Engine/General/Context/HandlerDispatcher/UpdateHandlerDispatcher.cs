using System;
using System.Reflection;

namespace Bro
{
    public class UpdateHandlerDispatcher : BaseHandlerDispatcher
    {
        public void AttachHandlersToObject(object target, IScheduler scheduler)
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
                        AttachUpdateHandler(target, cachedMethod, cachedAttribute, scheduler);
                    }
                }
            }
        }
        
        private static void AttachUpdateHandler(object target, MethodInfo method, object attribute, IScheduler scheduler)
        {
            var updateAttribute = (UpdateHandlerAttribute) attribute;

            if (method.GetParameters().Length == 0)
            {
                var action = Delegate.CreateDelegate(typeof(Action), target, method) as Action;
                scheduler.ScheduleUpdate(action, updateAttribute.UpdatePeriod);
            }
            else
            {
                Bro.Log.Error($"update function should have no parameters {target.GetType()} {method.Name}");
            }
        }
    }
}