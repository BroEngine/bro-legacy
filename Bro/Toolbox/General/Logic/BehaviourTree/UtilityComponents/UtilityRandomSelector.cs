using System.Collections.Generic;
using Bro;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class UtilityRandomSelector : Composite
    {
        private readonly List<UtilityNode> _children = new List<UtilityNode>();

        private int _activeChildIndex;
        private bool _needGenerateNewActiveChildIndex = true;
        
        private float[] _childrenUtilityValues;

        private readonly IRecheckUtility _recheckUtilityProvider;

        public UtilityRandomSelector(IRecheckUtility recheckProvider)
        {
            _recheckUtilityProvider = recheckProvider;
        }

        public override Result Process()
        {
            Result result;
            if (ChildrenCount == 0)
            {
                result = Result.Fail;
            }
            else
            {
                bool needReconsider = _recheckUtilityProvider.NeedRecheck;
                if (needReconsider || _needGenerateNewActiveChildIndex)
                {
                    GenerateNewActiveChildIndex();
                    _needGenerateNewActiveChildIndex = false;
                }

                ActiveNode = GetChildAtIndex(_activeChildIndex);
                result = ActiveNode.Process();

                switch (result)
                {
                    case Result.Success:
                    case Result.Fail:
                    case Result.Restart:
                        ActiveNode = null;
                        _needGenerateNewActiveChildIndex = true;
                        break;
                    case Result.Running:
                        break;

                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            return result;
        }

        public override void Reset()
        {
            base.Reset();
            _needGenerateNewActiveChildIndex = true;
        }

        private void GenerateNewActiveChildIndex()
        {
            if (_childrenUtilityValues == null || _children.Count != _childrenUtilityValues.Length)
            {
                _childrenUtilityValues = new float[_children.Count];
            }

            float childrenUtilitySum = 0f;
//            string debugStr = "Utility ";
            for (int i = 0, max = _children.Count; i < max; ++i)
            {
                var childUtility = _children[i].UtilityValue;
//                debugStr += i + ")" + childUtility + "; ";

                _childrenUtilityValues[i] = childUtility;
                childrenUtilitySum += childUtility;
            }

            var value = Bro.Random.Instance.Range(0f, childrenUtilitySum);
//            debugStr += " | " + value + " | ";
            for (int i = 0, max = _childrenUtilityValues.Length; i < max; ++i)
            {
                value -= _childrenUtilityValues[i];
                if (value <= 0f)
                {
                    _activeChildIndex = i;
                    break;
                }
            }

//            debugStr += " >>> " + _activeChildIndex;
//            Bro.Log.Info(debugStr);
        }

        public override void AddChild(Node node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException("Child cannot be null");
            }

            var utilityNode = node as UtilityNode;
            if (utilityNode == null)
            {
                throw new System.NotSupportedException("Can use only UtilityNode as child");
            }

            base.AddChild(node);
            _children.Add(utilityNode);
        }

        public override void RemoveChild(Node node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException("Child cannot be null");
            }

            var utilityNode = node as UtilityNode;
            if (utilityNode == null)
            {
                throw new System.NotSupportedException("Can use only UtilityNode as child");
            }

            _children.Remove(utilityNode);
            base.RemoveChild(node);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _recheckUtilityProvider.Reset();
        }

        public override void OnExit()
        {
            base.OnExit();
            ActiveNode = null;
            foreach (var child in _children)
            {
                child.Reset();
            }
        }
    }
}