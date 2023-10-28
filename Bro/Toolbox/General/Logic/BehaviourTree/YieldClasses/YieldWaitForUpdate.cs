namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class YieldWaitForUpdate: IBehaviourTreeYieldInstruction
    {
        private bool _isUpdateCalled;

        bool IBehaviourTreeYieldInstruction.IsFinished => _isUpdateCalled;

        public YieldWaitForUpdate()
        {
            _isUpdateCalled = false;
        }

        void IBehaviourTreeYieldInstruction.Tick()
        {
            _isUpdateCalled = true;
        }
            
        public void Reset()
        {
            _isUpdateCalled = false;
        }
    }
}