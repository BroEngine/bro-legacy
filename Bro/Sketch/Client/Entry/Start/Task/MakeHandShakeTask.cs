using System;
using Bro.Client;
using Bro.Client.Network;
using Bro.Network;

namespace Bro.Sketch.Client
{
    public class MakeHandShakeTask : SubscribableTask<MakeHandShakeTask>
    {
        private readonly NetworkEngine _networkEngine;
        private readonly IClientContext _context;
        private IDisposable _timeoutCheck;

        public MakeHandShakeTask(IClientContext context, NetworkEngine networkEngine)
        {
            _context = context;
            _networkEngine = networkEngine;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            Subscribe();
            var operation = new HandShakeOperation();
            _networkEngine.Send(operation);

            _timeoutCheck = _context.Scheduler.Schedule(() => Fail("timeout"), 5f);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Unsubscribe();
            _timeoutCheck?.Dispose();
            _timeoutCheck = null;
        }


        private void Subscribe()
        {
            _networkEngine.OnHandShakeOperationReceived += OnHandShakeOperationReceived;
            _networkEngine.OnStatusChanged += OnNetworkEngineStatusChanged;
        }

        private void Unsubscribe()
        {
            _networkEngine.OnHandShakeOperationReceived -= OnHandShakeOperationReceived;
            _networkEngine.OnStatusChanged -= OnNetworkEngineStatusChanged;
        }

        private void OnNetworkEngineStatusChanged(NetworkStatus status, int code)
        {
            switch (status)
            {
                case NetworkStatus.Disconnected:
                    Fail("server disconnect");
                    break;
            }
        }

        private void OnHandShakeOperationReceived(HandShakeOperation operation)
        {
            Complete();
        }
    }
}