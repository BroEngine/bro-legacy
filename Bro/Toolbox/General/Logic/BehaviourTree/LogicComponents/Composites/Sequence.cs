namespace Bro.Toolbox.Logic.BehaviourTree
{
    [BehaviourName("Sequence")]
    public class Sequence : Composite
    {
        private int _activeChildIndex = 0;

        public override void Reset()
        {
            _activeChildIndex = 0;
        }

        public override Result Process()
        {
            var result = Execute();
            return result;
        }

        public override void OnExit()
        {
            base.OnExit();
            ActiveNode = null;
        }

        private Result Execute()
        {
            if (ChildrenCount == 0)
            {
                return Result.Fail;
            }

            while (_activeChildIndex < ChildrenCount)
            {
                ActiveNode = GetChildAtIndex(_activeChildIndex);
                var result = ActiveNode.Process();
                switch (result)
                {
                    case Result.Success:
                        ++_activeChildIndex;
                        break;
                    case Result.Fail:
                        _activeChildIndex = 0;
                        return Result.Fail;
                    case Result.Running:
                        return Result.Running;
                    case Result.Restart:
                        return Result.Restart;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            ActiveNode = null;
            _activeChildIndex = 0;
            return Result.Success;
        }
    }
}