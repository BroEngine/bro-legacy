using System;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
    
    public class BehaviourContentAttribute: System.Attribute
    {
        public string ContentName { get; set; }
        
        public BehaviourContentAttribute(string content)
        {
            this.ContentName = content;
        }
    }
}