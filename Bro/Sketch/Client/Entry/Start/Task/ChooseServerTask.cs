using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Client.Network.Ping;

namespace Bro.Sketch.Client
{
    public class ChooseServerTask : SubscribableTask<ChooseServerTask>
    {
        private readonly IClientContext _context;
        private readonly List<IConnectionConfig> _availableServers;

        private int _serversChecked;
        private IConnectionConfig _bestServer;
        private long _bestPing = long.MaxValue;
        public IConnectionConfig ChosenServer => _bestServer;

        public ChooseServerTask(IClientContext context, List<IConnectionConfig> availableServers)
        {
            _availableServers = availableServers;
            _context = context;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            if (_availableServers.Count == 0)
            {
                Fail("no available servers");
            }
            else if (_availableServers.Count == 1)
            {
                _bestServer = _availableServers[0];
                Complete();
            }
            else
            {
                foreach (var server in _availableServers)
                {
                    var pingServerTask = new PingTask(_context, server, new NetworkEngine());
                    pingServerTask.OnComplete += (OnCompletePing);
                    pingServerTask.OnFail += (OnFinishServerPing);
                    pingServerTask.Launch(taskContext);
                }
            }
        }

        private void OnCompletePing(PingTask task)
        {
            if (task.PingResult < _bestPing)
            {
                _bestServer = task.Config;
            }
            OnFinishServerPing(task);
        }

        private void OnFinishServerPing(PingTask task)
        {
            ++_serversChecked;
            if (_serversChecked == _availableServers.Count)
            {
                if (_bestServer != null)
                {
                    Complete();
                }
                else
                {
                    Fail();
                }
            }
        }
    }
}