//using Bro;
//
//namespace Bro.Toolbox.Nodes.BehaviourTree
//{
//    public class RandomSingleSelector : Composite
//    {
//        private int _activeChildIndex;
//        private bool _needGenerateNewActiveChildIndex = true;
//        private readonly Random _random = new Random();
//        private readonly float[] _childrenChooseRatio;
//        private readonly float _childrenChooseSumRatio;
//
//        public RandomSingleSelector(float[] childrenChooseRatio = null)
//        {
//            _childrenChooseRatio = childrenChooseRatio;
//            if (_childrenChooseRatio != null)
//            {
//                for (int i = 0, max = _childrenChooseRatio.Length; i < max; ++i)
//                {
//                    _childrenChooseSumRatio += _childrenChooseRatio[i];
//                }
//            }
//        }
//
//        public override void Reset()
//        {
//            _needGenerateNewActiveChildIndex = true;
//        }
//
//        private void GenerateNewActiveChildIndex()
//        {
//            if (_childrenChooseRatio == null)
//            {
//                _activeChildIndex = _random.Range(0, ChildrenCount);
//            }
//            else
//            {
//#if UNITY_EDITOR
//                if (ChildrenCount != _childrenChooseRatio.Length)
//                {
//                }
//#endif
//
//                var value = _random.Range(0f, _childrenChooseSumRatio);
//                for (int i = 0, max = _childrenChooseRatio.Length; i < max; ++i)
//                {
//                    value -= _childrenChooseRatio[i];
//                    if (value <= 0f)
//                    {
//                        _activeChildIndex = i;
//                        break;
//                    }
//                }
//            }
//        }
//
//        public override Result Process()
//        {
//            Result result = Result.Fail;
//            if (ChildrenCount == 0)
//            {
//                result = Result.Fail;
//            }
//            else
//            {
//                if (_needGenerateNewActiveChildIndex)
//                {
//                    GenerateNewActiveChildIndex();
//                    _needGenerateNewActiveChildIndex = false;
//                }
//
//                result = GetChildAtIndex(_activeChildIndex).Process();
//
//                switch (result)
//                {
//                    case Result.Success:
//                    case Result.Fail:
//                    case Result.Restart:
//                        _needGenerateNewActiveChildIndex = true;
//                        break;
//                    case Result.Running:
//                        break;
//
//                    default:
//                        throw new System.ArgumentOutOfRangeException();
//                }
//            }
//
//            return result;
//        }
//    }
//}