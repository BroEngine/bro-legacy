using System;
using System.Collections;
using System.Diagnostics;
using Bro.Network;

namespace Bro.Client.Network
{
    internal static class RequestCounter
    {
        private static byte _counter = 0;

        public static byte Next => ++_counter;
    }

    public class SendRequestTask : SubscribableTask<SendRequestTask>
    {
        private readonly INetworkRequest _request;
        private readonly NetworkEngine _networkEngine;

        public SendRequestTask(INetworkRequest request, NetworkEngine networkEngine)
        {
            _request = request;
            _request.TemporaryIdentifier = RequestCounter.Next;
            _request.Retain();
            _networkEngine = networkEngine;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            if (_networkEngine.Send(_request))
            {
                Complete();
            }
            else
            {
                Fail();
            }
            _request.Release();
        }

        protected override void Start()
        {
            if (!_request.HasValidParams)
            {
                Bro.Log.Error("send request task :: request " + _request.GetType() + " has invalid params");
                return;
            }
            base.Start();
        }
    }

    public class SendRequestTask<T> : SubscribableTask<SendRequestTask<T>> where T : class, INetworkResponse
    {
        private readonly IClientContext _context;
        private readonly INetworkRequest _request;
        private IDisposable _responseTimeoutCoroutine;

        public bool IncreasedTimeout = false;
        private readonly NetworkEngine _networkEngine;
        public T Response { get; private set; }

        public SendRequestTask(IClientContext context, INetworkRequest request, NetworkEngine networkEngine)
        {
            _context = context;
            _request = request;
            _request.TemporaryIdentifier = RequestCounter.Next;
            _networkEngine = networkEngine;
        }

        protected override void Start()
        {
            if (!_request.HasValidParams)
            {
                Bro.Log.Error("send request task :: request " + _request.GetType() + " has invalid params");
                return;
            }
            base.Start();
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            Subscribe();

            if (!_networkEngine.Send(_request))
            {
                ProcessFail();
            }
        }

        private void ProcessComplete(T response)
        {
            if (response == null)
            {
                ProcessFail();
            }
            else
            {
                Unsubscribe();
                Response = response;
                Complete();
            }
        }

        private void ProcessFail()
        {
            Unsubscribe();
            Fail();
        }

        protected override void ProcessOnTerminate()
        {
            Unsubscribe();
            base.ProcessOnTerminate();
        }

        private void Subscribe()
        {
            _request.Retain();
            
            _networkEngine.OnNetworkResponseReceived += OnResponse;
            _responseTimeoutCoroutine = _context.Scheduler.StartCoroutine(TimeoutCoroutine(IncreasedTimeout ? NetworkConfig.ResponseIncreasedTimeout : NetworkConfig.ResponseTimeout));
        }

        private void Unsubscribe()
        {
            _request.Release();
            
            _networkEngine.OnNetworkResponseReceived -= OnResponse;

            if (_responseTimeoutCoroutine != null)
            {
                _responseTimeoutCoroutine.Dispose();
                _responseTimeoutCoroutine = null;
            }
        }

        private void OnResponse(INetworkResponse response)
        {
            if (response.OperationCode == _request.OperationCode && response.TemporaryIdentifier == _request.TemporaryIdentifier)
            {
                ProcessComplete(response as T);
            }
        }

        private IEnumerator TimeoutCoroutine(long timeout)
        {
            var timer = new Stopwatch();
            timer.Start();

            while (true)
            {
                yield return new Timing.YieldWaitForEndOfFrame();

                if (!_networkEngine.IsConnected())
                {
                    break;
                }
                if (timer.ElapsedMilliseconds > timeout)
                {
                    Log.Error($"send request task :: request {typeof(T)} failed after timeout type = {timeout}");
                    break;
                }
            }

            ProcessFail();
        }
    }
}