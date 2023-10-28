using System.Collections;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class CoroutineNode : Node
    {
        private bool _isFinished;
        private bool _isSuccessful = true;
        private IBehaviourTreeYieldInstruction _lastYieldInstruction;
        private IEnumerator _enumerator;
        protected abstract IEnumerator CoroutineFunction();
        
        protected void SetResult(bool isSuccess)
        {
            _isSuccessful = isSuccess;
            _isFinished = true;
        }
        
        public override Result Process()
        {
            OnProcess();
            if (_isFinished)
            {
                return _isSuccessful ? Result.Success : Result.Fail;
            }
            return Result.Running;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _lastYieldInstruction = null;
            _enumerator = CoroutineFunction();
        }

        public override void OnExit()
        {
            base.OnExit();
            _lastYieldInstruction = null;
            _enumerator = null;
            _isFinished = false;
        }

        private void OnProcess()
        {
            if (_enumerator == null || _isFinished)
            {
                return;
            }

            if (_lastYieldInstruction != null)
            {
                _lastYieldInstruction.Tick();

                if (!_lastYieldInstruction.IsFinished)
                {
                    return;
                }
            }

            _lastYieldInstruction = null;

            if (_enumerator.MoveNext())
            {
                if (_enumerator.Current is IBehaviourTreeYieldInstruction t)
                {
                    _lastYieldInstruction = t;
                }
            }
            else
            {
                _isFinished = true;
            }
        }
        
    }
    
}