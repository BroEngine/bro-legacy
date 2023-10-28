using System;
using System.Collections.Generic;

namespace Bro
{
    public class SubscribableTask<T> : Task where T : Task
    {
        public event Action<T> OnComplete;
        public event Action<T> OnFail;
        public event Action<T> OnTerminate;
        
        private void ClearHandlers()
        {
            OnComplete = null;
            OnFail = null;
            OnTerminate = null;
        }

        protected override void ProcessOnComplete()
        {
            base.ProcessOnComplete();
            OnComplete?.Invoke((T) (this as Task));
            ClearHandlers();
        }

        protected override void ProcessOnFail()
        {
            base.ProcessOnFail();
            OnFail?.Invoke((T) (this as Task));
            ClearHandlers();
        }

        protected override void ProcessOnTerminate()
        {
            base.ProcessOnTerminate();
            OnTerminate?.Invoke((T) (this as Task));
            ClearHandlers();
        }
    }
}