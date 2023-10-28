namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForTask<T> : IYieldInstruction where T : Bro.Task
        {
            public enum State
            {
                TaskExecuting,
                TaskCompleted,
                TaskFailed,
                TaskTerminated
            }

            public State Result { get; private set; }

            private readonly Bro.SubscribableTask<T> _task;

            public YieldWaitForTask(Bro.SubscribableTask<T> task)
            {
                Result = State.TaskExecuting;
                _task = task;
                _task.OnComplete += (OnTaskComplete);
                _task.OnFail += (OnTaskFailed);
                _task.OnFail += (OnTaskTerminated);
            }

            private void OnTaskComplete(T task)
            {
                Result = State.TaskCompleted;
            }

            private void OnTaskFailed(T task)
            {
                Result = State.TaskFailed;
            }
            
            private void OnTaskTerminated(T task)
            {
                Result = State.TaskTerminated;
            }

            void IYieldInstruction.Tick(TickType tickType)
            {
            }

            bool IYieldInstruction.IsFinished => Result != State.TaskExecuting;

            public bool IsTaskTerminated => Result == State.TaskTerminated;
            
            public bool IsTaskFailed => Result == State.TaskFailed;

            public bool IsTaskCompleted => Result == State.TaskCompleted;
            
            public void Reset()
            {
                 throw new System.NotImplementedException();
            }
        }
    }
}