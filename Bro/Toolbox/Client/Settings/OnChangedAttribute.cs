using System;
using UnityEngine;

namespace Bro.Toolbox
{
    public class OnChangedAttribute : PropertyAttribute
    {
        public readonly string MethodName;
        
        public OnChangedAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}