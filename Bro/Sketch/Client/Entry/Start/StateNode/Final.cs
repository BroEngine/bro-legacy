using System;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class Final:EndPoint
    {
        private Action _onComplete;
        public Final(Action onComplete)
        {
            _onComplete = onComplete;
        }
        public override Result Process()
        {
            Bro.Log.Info("final node :: Final");
            _onComplete?.Invoke();
            return Result.Running;
        }
    }
}