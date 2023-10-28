namespace Bro.Client
{
    public partial class Timing
    {
        public interface IYieldInstruction
        {
            void Tick(TickType tickType);
            void Reset();
            bool IsFinished { get; }
        }

        public enum TickType
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}