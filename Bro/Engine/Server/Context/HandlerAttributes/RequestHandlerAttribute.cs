using System;

namespace Bro.Server.Context
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestHandlerAttribute : Attribute
    {
        public readonly byte OperationCode;

        public RequestHandlerAttribute(byte operationCode)
        {
            OperationCode = operationCode;
        }
    }
}