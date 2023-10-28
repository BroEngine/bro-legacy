using System;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public abstract class TaskRunner<T> : EndPoint where T : SubscribableTask<T>
    {
        private Result _result;
        public T Task { get; private set; }

        #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT
        private readonly Bro.Client.IClientContext _context;
        #endif
        
        protected abstract T CreateTask();

        #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT
        protected TaskRunner(Bro.Client.IClientContext context)
        {
            _context = context;
        }
        #endif

        public override void OnEnter()
        {
            base.OnEnter();
            _result = Result.Running;
            Task = CreateTask();
            Task.OnComplete += (t => _result = Result.Success);
            Task.OnFail += (t => _result = Result.Fail);
            Task.OnTerminate += (t => _result = Result.Fail);
            
            #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT
            Task.Launch(_context);
            #else
            throw new NotImplementedException("need task context here");
            #endif
        }

        public override void OnExit()
        {
            base.OnExit();
            if (Task.IsRunning)
            {
                Task.Terminate();
            }
            Task = null;
        }

        public override Result Process()
        {
            return _result;
        }
    }
}