using System;

namespace Bro.Network.TransmitProtocol
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    
    public class TypeBinderAttribute: Attribute
    {
        public readonly string TypeKey;

        public TypeBinderAttribute(string key)
        {
            TypeKey = key;
        }
    }
}