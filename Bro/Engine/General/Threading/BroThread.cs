using System;
using System.Threading;

namespace Bro.Threading
{
    public class BroThread
    {
        private readonly Thread _thread;
        
        public bool IsBackground { set => _thread.IsBackground = value; }

        public ThreadPriority Priority { set => _thread.Priority = value; }

        public string Name { set => _thread.Name = value; }
        
        public BroThread(ParameterizedThreadStart start)
        {
            _thread = new Thread(start);
            SubscribeOnExit();
        }

        public BroThread(ThreadStart start)
        {
            _thread = new Thread(start);
            SubscribeOnExit();
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Start(object parameter)
        {
            _thread.Start(parameter);
        }

        public void Join()
        {
            _thread.Join();
        }

        public bool Join(int millisecondsTimeout)
        {
            return _thread.Join(millisecondsTimeout);
        }

        public void Abort()
        {
            if (_thread != null)
            {
                _thread.Abort();
                UnsubscribeOnExit();
            }
        }

        private void SubscribeOnExit()
        {
            AssemblyProcessExit.ProcessExit += OnApplicationExit;
        }

        private void UnsubscribeOnExit()
        {
            AssemblyProcessExit.ProcessExit -= OnApplicationExit;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            Abort();
        }
    }
}