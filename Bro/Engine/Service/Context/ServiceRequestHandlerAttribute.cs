using System;

namespace Bro.Service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceRequestHandlerAttribute : Attribute
    {
        public readonly byte OperationCode;

        public ServiceRequestHandlerAttribute(byte operationCode)
        {
            OperationCode = operationCode;
        }
    }
}