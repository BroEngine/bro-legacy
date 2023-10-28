namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForFixedUpdate : IYieldInstruction
        {
            private bool _isUpdateCalled;

            void IYieldInstruction.Tick(TickType tickType)
            {
                if (tickType == TickType.FixedUpdate)
                {
                    _isUpdateCalled = true;
                }
            }

            bool IYieldInstruction.IsFinished => _isUpdateCalled;
            public void Reset()
            {
                _isUpdateCalled = false;
            }
            
        }
    }
}