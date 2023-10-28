using System;

namespace Bro.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceEventHandlerAttribute  : Attribute
    {
        public readonly byte OperationCode;

        public ServiceEventHandlerAttribute(byte operationCode)
        {
            OperationCode = operationCode;
        }
    }
}