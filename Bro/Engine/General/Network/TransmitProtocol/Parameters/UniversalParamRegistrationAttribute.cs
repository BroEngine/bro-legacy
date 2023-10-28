using System;

namespace Bro.Network.TransmitProtocol
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UniversalParamRegistrationAttribute: System.Attribute
    {
        public readonly Type Type;
        public readonly byte ParamTypeIndex;

        public UniversalParamRegistrationAttribute(Type type, byte index)
        {
            Type = type;
            ParamTypeIndex = index;
        }
    }

   
}