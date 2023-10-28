using System;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    [AttributeUsage(AttributeTargets.Class)]
    
    public class BehaviourNameAttribute: System.Attribute
    {
        public string NodeName { get; set; }
        
        public BehaviourNameAttribute(string name)
        {
            this.NodeName = name;
        }
    }
}