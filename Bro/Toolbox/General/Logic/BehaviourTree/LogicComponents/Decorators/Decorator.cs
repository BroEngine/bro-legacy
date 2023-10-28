using System.Collections;
using System.Collections.Generic;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class Decorator : Node, IEnumerable
    {
        #region IEnumerable implementation

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Add(Node node)
        {
            AddChild(node);
        }

        #endregion

        public override IEnumerable<Node> Children
        {
            get { return new Node[] {_child}; }
        }

        private Node _child;

        public Node Child
        {
            get { return _child; }
        }

        public override void AddChild(Node node)
        {
            if (_child == node)
            {
                return;
            }

            if (_child != null)
            {
                return;
            }

            base.AddChild(node);
            _child = node;
        }

        public override void RemoveChild(Node node)
        {
            if (_child != node)
            {
                return;
            }

            base.RemoveChild(node);
            _child = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _child?.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            _child?.OnExit();
        }

        public override void Reset()
        {
            _child?.Reset();
        }
    }
}