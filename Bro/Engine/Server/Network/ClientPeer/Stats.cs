using System.Collections.Generic;
using System.Diagnostics;
using Bro.Network;

namespace Bro.Server
{
    public class Stats
    {
        private readonly Dictionary<byte, int> _operationsRecieved = new Dictionary<byte, int>();
        private readonly Dictionary<byte, int> _operationsSent = new Dictionary<byte, int>();

        private readonly Stopwatch _lifetimer = new Stopwatch();
        private long _totalBytesReceived; //statistics on the recieved bytes
        private long _totalBytesSent; // statistics on the sent bytes
        public long Lifetime { get { return _lifetimer.ElapsedMilliseconds; } }

        private readonly Stopwatch _statsTimer = new Stopwatch();
        private const long LogPeriodInMs = 3000L;
        private const string UsingDefine = "SERVER_STATS";
        private readonly int _peerId;

        public Stats(int peerId)
        {
            _peerId = peerId;
            _statsTimer.Start();
            _lifetimer.Start();
        }

        [Conditional(UsingDefine)]
        public void OnSendBytes(int bytes)
        {
            _totalBytesSent += bytes;
        }

        [Conditional(UsingDefine)]
        public void OnReceiveBytes(int bytes)
        {
            _totalBytesReceived += bytes;
        }

        [Conditional(UsingDefine)]
        public void OnSendOperation(INetworkOperation operation)
        {
            lock (_operationsSent)
            {
                if (!_operationsSent.ContainsKey(operation.OperationCode))
                {
                    _operationsSent[operation.OperationCode] = 0;
                }
                _operationsSent[operation.OperationCode] += 1;
            }
        }

        [Conditional(UsingDefine)]
        public void Log()
        {
            if (_statsTimer.ElapsedMilliseconds > LogPeriodInMs)
            {
                _statsTimer.Stop();

                string d = "PEER = " + _peerId + "; TIME = " + _statsTimer.ElapsedMilliseconds + "; \n SEND: " + _totalBytesSent + " bytes \n";
                lock (_operationsSent)
                {
                    foreach (var send in _operationsSent)
                    {
                        d += "[ " + send.Key + " ] = " + send.Value + "; ";
                    }
                    _operationsSent.Clear();
                    _totalBytesSent = 0;
                }

                d += "\n RCVD:  " + _totalBytesReceived + " bytes \n";
                lock (_operationsRecieved)
                {
                    foreach (var rec in _operationsRecieved)
                    {
                        d += "[ " + rec.Key + " ] = " + rec.Value + "; ";
                    }
                    _operationsRecieved.Clear();
                    _totalBytesReceived = 0;
                }
                Bro.Log.Info(d);
                _statsTimer.Reset();
                _statsTimer.Start();
            }
        }

        [Conditional(UsingDefine)]
        public void OnReceiveOperation(INetworkOperation operation)
        {
            lock (_operationsRecieved)
            {
                if (!_operationsRecieved.ContainsKey(operation.OperationCode))
                {
                    _operationsRecieved[operation.OperationCode] = 0;
                }
                _operationsRecieved[operation.OperationCode] += 1;
            }
        }
    }
}