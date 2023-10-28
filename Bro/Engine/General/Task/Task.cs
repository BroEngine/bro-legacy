using System.Collections.Generic;

namespace Bro
{
    public class Task
    {
        private static readonly List<Task> ActiveTasks = new List<Task>();

        private enum State
        {
            Created,
            Started,
            Active,
            Completed,
            Failed,
            Terminated
        }

        private State _currentState;
        protected ITaskContext TaskContext { get; private set; }
        
        public object FailReason { get; private set; }

        public bool IsRunning => _currentState == State.Active; 

        protected Task()
        {
            _currentState = State.Created;
        }

        public void Launch(ITaskContext taskContext)
        {
            TaskContext = taskContext;
            TaskContext?.Add(this);
            lock (ActiveTasks)
            {
                ActiveTasks.Add(this);
            }
            Start();
        }

        protected virtual void Start()
        {
            if (_currentState != State.Created)
            {
                Bro.Log.Error("task :: state is not valid; currentState = " + _currentState + " task = " + GetType());
            }
            _currentState = State.Started;
            Activate(TaskContext);
        }

        protected virtual void Activate(ITaskContext taskContext)
        {
            if (_currentState != State.Started)
            {
                Bro.Log.Error("task :: state is not valid; currentState = " + _currentState + " task = " + GetType());
            }
            _currentState = State.Active;
        }

        protected void Complete()
        {
            if (_currentState != State.Active && _currentState != State.Terminated)
            {
                Bro.Log.Error("task :: state is not valid; currentState = " + _currentState + " task = " + GetType());
            }

            if (_currentState == State.Terminated)
            {
                return;
            }

            _currentState = State.Completed;
            ProcessOnComplete();
            Destroy();
        }

        protected void Fail(object failReason = null)
        {
            if (_currentState != State.Active && _currentState != State.Terminated)
            {
                Bro.Log.Error("task :: state is not valid; currentState = " + _currentState + " task = " + GetType());
            }

            if (_currentState == State.Terminated)
            {
                return;
            }
            
            FailReason = failReason;
            _currentState = State.Failed;
            ProcessOnFail();
            Destroy();
        }
        
        public void Terminate()
        {
            if (_currentState == State.Terminated)
            {
                return;
            }
            
            _currentState = State.Terminated;
            ProcessOnTerminate();
            Destroy();
        }

        protected virtual void ProcessOnComplete()
        {
        }

        protected virtual void ProcessOnFail()
        {
        }
        
        protected virtual void ProcessOnTerminate()
        {
        }
        
        protected virtual void OnDestroy()
        {
        }

        private void Destroy()
        {
            OnDestroy();
            lock (ActiveTasks)
            {
                ActiveTasks.FastRemove(this);
            }
            TaskContext?.Remove(this);
        }
    }
}