
using System.Collections;
using System.Collections.Generic;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class Composite : Node, IEnumerable
    {
        public override IEnumerable<Node> Children{
            get { return _children; }
        }  

        private readonly List<Node> _children = new List<Node>();

        public int ChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }  
        
        private Node _activeNode;
        protected Node ActiveNode
        {
            get
            {
                return _activeNode;
            }
            set
            {
                if (_activeNode == value)
                {
                    return;
                }

                if (_activeNode != null)
                {
                    _activeNode.OnExit();
                }
                
                _activeNode = value;
                if (_activeNode != null)
                {
                    _activeNode.OnEnter();
                }
            }
        }

        public Node GetChildAtIndex(int index)
        {
            return _children[index];
        }

        public override void AddChild(Node node)
        {
            base.AddChild(node);
            _children.Add(node);
        }

        public override void RemoveChild(Node node)
        {
            bool hadChildInside = _children.Remove(node);
            if (hadChildInside)
            {
                base.RemoveChild(node);
            }
        }

        public override void Reset()
        {
            ActiveNode = null;
        }

        #region IEnumerable implementation

        public IEnumerator GetEnumerator()
        {
            throw new System.NotSupportedException();
        }

        public void Add(Node node)
        {
            AddChild(node);
        }

        public void Add(Node node1, Node node2)
        {
            AddChild(node1);
            AddChild(node2);
        }

        public void Add(Node node1, Node node2, Node node3)
        {
            AddChild(node1);
            AddChild(node2);
            AddChild(node3);
        }

        public void Add(Node node1, Node node2, Node node3, Node node4)
        {
            AddChild(node1);
            AddChild(node2);
            AddChild(node3);
            AddChild(node4);
        }

        public void Add(Node node1, Node node2, Node node3, Node node4, Node node5)
        {
            AddChild(node1);
            AddChild(node2);
            AddChild(node3);
            AddChild(node4);
            AddChild(node5);
        }

        #endregion
    }
}