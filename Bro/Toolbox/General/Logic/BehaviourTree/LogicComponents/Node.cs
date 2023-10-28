
using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Toolbox.Node;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class Node : IBehaviourNode
    {
#if UNITY_EDITOR
        private static int IdCounter;
        public readonly int Id;
#endif

        public bool IsActive;

        public Node Parent { get; private set; }

        public abstract Result Process();

        public abstract IEnumerable<Node> Children { get; }

        protected Node()
        {
#if UNITY_EDITOR
            Id = Interlocked.Increment(ref IdCounter);
            #endif
        }

        #region IBehaviourNode implementation

        public virtual void OnEnter()
        {
            IsActive = true;
        }

        public virtual void OnExit()
        {
            IsActive = false;
        }

        public virtual void Update()
        {
            Process();
        }

        public virtual void Reset()
        {
        }

        #endregion
        
        
        public virtual void AddChild(Node child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child cannot be null");
            }

            child.Parent = this;
        }

        public virtual void RemoveChild(Node child)
        {
            child.Parent = null;
        }
        
    }
}