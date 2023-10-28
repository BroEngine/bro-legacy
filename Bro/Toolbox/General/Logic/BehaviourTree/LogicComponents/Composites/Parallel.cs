using System;
using System.Collections.Generic;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    [BehaviourName("Parallel")]
    public class Parallel : Composite
    {
        private readonly List<Node> _activeNodes = new List<Node>();

        public override void AddChild(Node node)
        {
            base.AddChild(node);
            _activeNodes.Add(null);
        }

        public override void RemoveChild(Node node)
        {
            base.RemoveChild(node);
            _activeNodes.RemoveAt(_activeNodes.Count - 1);
        }

        private Node GetActiveNode(int childIndex)
        {
            return _activeNodes[childIndex];
        }

        private void SetActiveNode(int childIndex, Node value)
        {
            if (_activeNodes[childIndex] == value)
            {
                return;
            }

            
            if (_activeNodes[childIndex] != null)
            {
                _activeNodes[childIndex].OnExit();
            }
            _activeNodes[childIndex] = value;
            if (_activeNodes[childIndex] != null)
            {
                _activeNodes[childIndex].OnEnter();
            }
        }

        private void AnnulAllActiveNodes()
        {
            for (int i = 0, max = ChildrenCount; i < max; ++i)
            {
                if (_activeNodes[i] != null)
                {
                    _activeNodes[i].Reset();
                }
                SetActiveNode(i, null);
            }
        }

        public override void Reset()
        {
            AnnulAllActiveNodes();
        }

        public override void OnExit()
        {
            base.OnExit();
            AnnulAllActiveNodes();
        }

        public override Result Process()
        {
            Result result = Result.Fail;
            if (ChildrenCount == 0)
            {
                result = Result.Fail;
            }
            else
            {
                for (int i = 0, max = ChildrenCount; i < max; ++i)
                {
                    var activeNode = GetChildAtIndex(i);
                    SetActiveNode(i, activeNode);
                    result = activeNode.Process();
                    if (result == Result.Fail || result == Result.Restart || result == Result.Success)
                    {
                        AnnulAllActiveNodes();
                        break;
                    }
                }
            }

            return result;
        }
    }
}