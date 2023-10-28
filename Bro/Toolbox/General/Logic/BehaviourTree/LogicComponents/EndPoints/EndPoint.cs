using System.Collections.Generic;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class EndPoint : Node
    {
        public override IEnumerable<Node> Children => null;

        public override void AddChild(Node child)
        {
            throw new System.InvalidOperationException("Cannot add child to end point");
        }

        public override void RemoveChild(Node child)
        {
            throw new System.InvalidOperationException("Cannot remove child from end point");
        }
    }
}