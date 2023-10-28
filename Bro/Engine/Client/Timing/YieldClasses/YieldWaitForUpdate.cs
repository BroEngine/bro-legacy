namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForUpdate : IYieldInstruction
        {
            private bool _isUpdateCalled;

            bool IYieldInstruction.IsFinished => _isUpdateCalled;

            public YieldWaitForUpdate()
            {
                _isUpdateCalled = false;
            }

            void IYieldInstruction.Tick(TickType tickType)
            {
                if (tickType == TickType.Update)
                {
                    _isUpdateCalled = true;
                }
            }
            
            public void Reset()
            {
                _isUpdateCalled = false;
            }
        }
    }
}