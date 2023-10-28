namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForEndOfFrame : IYieldInstruction
        {
            private bool _isLateUpdateCalled;

            bool IYieldInstruction.IsFinished => _isLateUpdateCalled;

            void IYieldInstruction.Tick(TickType tickType)
            {
                if (tickType == TickType.LateUpdate)
                {
                    _isLateUpdateCalled = true;
                }
            }

            public void Reset()
            {
                _isLateUpdateCalled = false;
            }
        }
    }
}