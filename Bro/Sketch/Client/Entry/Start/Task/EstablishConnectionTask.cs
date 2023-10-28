using System;
using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class EstablishConnectionTask : SubscribableTask<EstablishConnectionTask>
    {
        private readonly IClientContext _context;
        private readonly IConnectionConfig _connectionConfig;
        private readonly NetworkEngine _networkEngine;
        private IDisposable _timeoutCheck;

        public EstablishConnectionTask(IClientContext context, IConnectionConfig connectionConfig, NetworkEngine networkEngine)
        {
            _context = context;
            _networkEngine = networkEngine;
            _connectionConfig = connectionConfig;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            
            _timeoutCheck = _context.Scheduler.Schedule(() => Fail("timeout"), 5f);
            
            _networkEngine.OnStatusChanged += OnChangeNetworkStatusChanged;
            _networkEngine.Connect(_context.Application.GlobalContext, _connectionConfig);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _timeoutCheck?.Dispose();
            _timeoutCheck = null;
            _networkEngine.OnStatusChanged -= OnChangeNetworkStatusChanged;
        }

        private void OnChangeNetworkStatusChanged(NetworkStatus status, int disconnectCode)
        {
            switch (status)
            {
                case NetworkStatus.Disconnected:
                    Fail("disconnect");
                    break;
                case NetworkStatus.Connected:
                    Complete();
                    break;
            }
        }

        protected override void ProcessOnTerminate()
        {
            base.ProcessOnTerminate();
            _networkEngine.Disconnect(0);
        }
        
    }
}