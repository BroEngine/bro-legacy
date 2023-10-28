namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForAction : IYieldInstruction
        {
            private bool _isFinished;
            
            public void Callback()
            {
                _isFinished = true;
            }

            public void Tick(TickType tickType) { }

            public void Reset() { }

            public bool IsFinished => _isFinished;
        }
        
        public class YieldWaitForAction<T> : IYieldInstruction
        {
            public T Result { get; private set; }
            private bool _isFinished;
            
            public void Callback(T result)
            {
                Result = result;
                _isFinished = true;
            }

            public void Tick(TickType tickType) { }

            public void Reset() { }

            public bool IsFinished => _isFinished;
        }
    }
}