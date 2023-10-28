// ReSharper disable ObjectCreationAsStatement

using System;
using System.Collections.Generic;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using Bro.Client;

#endif


namespace Bro
{
    public class SequenceTask : SubscribableTask<SequenceTask>
    {
        private int _currentTaskIndex = 0;
        private readonly List<Task> _tasks = new List<Task>();

        public event Action<Task, int, int> OnChildCompleted;

        private readonly ITaskContext _taskContext;

        #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
        private readonly Timing.IScheduler _scheduler; // on client it always TimingScheduler
        public SequenceTask(IClientContext context) 
        {
            _scheduler = context.Scheduler;
            _taskContext = context;
        }
        #endif
        public SequenceTask(ITaskContext taskContext)
        {
            _taskContext = taskContext;
        }
        //#endif
        
        public bool IsEmpty()
        {
            return _tasks.Count == 0;
        }

        public void Add<T>(Bro.SubscribableTask<T> task) where T : Task
        {
            _tasks.Add(task);
            task.OnComplete += (t => OnFinishChildTask());
            task.OnTerminate += (t => Terminate());
            task.OnFail += (t => Fail());
        }

        private void OnFinishChildTask()
        {
            OnChildCompleted?.Invoke(_tasks[_currentTaskIndex], _currentTaskIndex, _tasks.Count);
            ++_currentTaskIndex;
            LaunchChildTask();
        }

        private void LaunchChildTask()
        {
            #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            if (_scheduler != null)
            {
                _scheduler.Schedule(ProcessChildTask);
            }
            else
            #endif
            {
                ProcessChildTask();
            }
        }

        private void ProcessChildTask()
        {
            if (_currentTaskIndex < _tasks.Count)
            {
                _tasks[_currentTaskIndex].Launch(_taskContext);
            }
            else
            {
                Complete();
            }
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            LaunchChildTask();
        }
    }
}