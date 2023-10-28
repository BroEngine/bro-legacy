using System;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class LoadingProgress : EndPoint
    {
        private readonly Action _action;

        public LoadingProgress(Action action)
        {
            _action = action;
        }

        public override Result Process()
        {
            _action?.Invoke();
            return Result.Success;
        }
    }
}