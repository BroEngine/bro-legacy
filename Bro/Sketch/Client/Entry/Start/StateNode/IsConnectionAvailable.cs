using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class IsConnectionAvailable:Condition
    {
        public IsConnectionAvailable(PredicateDelegate handler)
        {
            ConditionHandler = handler;
        }
        
    }
}