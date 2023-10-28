using System;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class Condition : Composite
    {
        public delegate bool PredicateDelegate();

        protected PredicateDelegate ConditionHandler;

        public Condition()
        {
            
        }
        public Condition(PredicateDelegate conditionHandler)
        {
            ConditionHandler = conditionHandler;
        }

        private bool HasNodeForFalseResult()
        {
            return (ChildrenCount > 1);
        }

        private Node NodeForFalseResult {get{return GetChildAtIndex(1);}}

        private bool HasNodeForTrueResult()
        {
            return (ChildrenCount > 0);
        }

        private Node NodeForTrueResult {get{return GetChildAtIndex(0);}} 

        public override void OnExit()
        {
            base.OnExit();
            ActiveNode = null;
        }

        public override Result Process()
        {
            Result result;
            if (ConditionHandler())
            {
                if (HasNodeForTrueResult())
                {
                    ActiveNode = NodeForTrueResult;
                    result = ActiveNode.Process();
                }
                else
                {
                    result = Result.Success;
                }
            }
            else
            {
                if (HasNodeForFalseResult())
                {
                    ActiveNode = NodeForFalseResult;
                    result = ActiveNode.Process();
                }
                else
                {
                    result = Result.Fail;
                }
            }

            return result;
        }

        public override void AddChild(Node node)
        {
            if (ChildrenCount < 2)
            {
                base.AddChild(node);
            }
            else
            {
                throw new NotSupportedException("Cannot contain more than 2 nodes");
            }
        }
    }
}