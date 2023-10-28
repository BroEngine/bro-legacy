using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;


namespace Bro.Sketch.Client
{
    public class TimeSyncTask : SubscribableTask<TimeSyncTask>
    {
        public long TimestampWhenServerReceivedRefresh { get; private set; }

        private readonly IClientContext _context;
        private readonly NetworkEngine _networkEngine;

        public TimeSyncTask(IClientContext context, NetworkEngine networkEngine)
        {
            _context = context;
            _networkEngine = networkEngine;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            var request = NetworkPool.GetOperation<TimeSyncRequest>();
            var sendRequest = new SendRequestTask<TimeSyncResponse>(_context, request, _networkEngine);
            sendRequest.OnComplete += (OnSendRequestTaskCompleted);
            sendRequest.OnFail += (OnSendRequestTaskFailed);
            sendRequest.OnTerminate += (OnSendRequestTaskFailed);
            sendRequest.Launch(taskContext);
        }

        private void OnSendRequestTaskCompleted(SendRequestTask<TimeSyncResponse> task)
        {
            TimestampWhenServerReceivedRefresh = task.Response.ServerTimestamp.Value;
            Complete();
        }

        private void OnSendRequestTaskFailed(SendRequestTask<TimeSyncResponse> task)
        {
            Fail(task.FailReason);
        }
    }
}