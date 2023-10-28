namespace Bro.Toolbox.Logic.BehaviourTree
{
    public interface IBehaviourTreeYieldInstruction
    {
        void Reset();
        bool IsFinished { get; }
        void Tick();
    }
}