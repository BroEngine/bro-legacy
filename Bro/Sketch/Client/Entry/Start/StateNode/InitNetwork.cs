using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class InitNetwork : Parallel
    {
        readonly Stopwatch _timer = new Stopwatch();
        private const long MaxNetworkEstablishTime = 6000L;
        private const long InformNetworkEstablishTime = 4000L;

        public InitNetwork(Condition.PredicateDelegate connectionStatusHandler, Action informHandler)
        {
            var maxNetworkEstablishTimeout = new Timeout(_timer, MaxNetworkEstablishTime);
            var informNetworkEstablishTimeout = new Timeout(_timer, InformNetworkEstablishTime, informHandler);
            var checkConnection = new IsConnectionAvailable(connectionStatusHandler);
            checkConnection.AddChild(new ResultSetter(Result.Success));
            checkConnection.AddChild(new ResultSetter(Result.Running));
            base.AddChild(maxNetworkEstablishTimeout);
            base.AddChild(informNetworkEstablishTimeout);
            base.AddChild(checkConnection);
        }

        public override Result Process()
        {
            if (!_timer.IsRunning)
            {
                _timer.Restart();
            }

            return base.Process();
        }
    }
}