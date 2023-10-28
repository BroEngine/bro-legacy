using System.Diagnostics;
using Bro.Network;

namespace Bro.Client.Network.Ping
{
    public class PingTask : SubscribableTask<PingTask>
    {
        public long PingResult
        {
            get
            {
                if (_iterationsCompleted > 0)
                {
                    return _requestTotalTime / _iterationsCompleted / 2;
                }

                return 0;
            }
        }

        private long _requestTotalTime;
        public readonly IConnectionConfig Config;

        private readonly NetworkEngine _engine;

        private readonly Stopwatch _timer;
        private readonly int _iterations;
        private int _iterationsCompleted;
        private bool _isPingCompleted;

        private readonly IClientContext _context;

        public PingTask(IClientContext context, IConnectionConfig config, NetworkEngine networkEngine, int iterations = 3)
        {
            Config = config;
            _context = context;
            _iterations = iterations;
            _timer = new Stopwatch();
            _engine = networkEngine;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);

            if (_engine.IsConnected())
            {
                Bro.Log.Info("ping task :: task can not be done because there is active connection to the remote server.");
                ProcessFail();
                return;
            }

            if (_iterations < 1)
            {
                Bro.Log.Info("ping task ::  ask can not be done because iterations sets to = " + _iterations);
                ProcessFail();
                return;
            }

            Subscribe();
            _engine.Connect(_context, Config);
        }

        private void Subscribe()
        {
            _engine.OnPingOperationReceived += OnPingOperationEvent;
            _engine.OnStatusChanged += OnStatusChanged;
        }

        private void Unsubscribe()
        {
            _engine.OnPingOperationReceived -= OnPingOperationEvent;
            _engine.OnStatusChanged -= OnStatusChanged;
        }

        private void OnStatusChanged(NetworkStatus status, int code)
        {
            switch (status)
            {
                case NetworkStatus.Connected:
                    ProcessPing();
                    break;
                case NetworkStatus.Disconnected:

                    if (!_isPingCompleted)
                    {
                        Bro.Log.Info("ping task :: can not be done because communication with the server was gone");
                        ProcessFail();
                    }
                    else
                    {
                        ProcessComplete();
                    }
                    break;
            }
        }

        private void ProcessComplete()
        {
            _timer.Stop();
            Unsubscribe();
            Complete();
        }

        private void ProcessFail()
        {
            _timer.Stop();
            Unsubscribe();
            Fail();
        }

        protected override void ProcessOnTerminate()
        {
            _timer.Stop();
            Unsubscribe();
            base.ProcessOnTerminate();
        }

        private void ProcessPing()
        {
            _engine.Send(new PingOperation());
            _timer.Reset();
            _timer.Start();
        }

        private void OnPingOperationEvent(PingOperation e)
        {
            ++_iterationsCompleted;
            _requestTotalTime += _timer.ElapsedMilliseconds;

            if (_iterationsCompleted == _iterations)
            {
                _isPingCompleted = true;
                _engine.Disconnect(DisconnectCode.ClientLeaveAfterPing);
            }
            else
            {
                ProcessPing();
            }
        }
    }
}