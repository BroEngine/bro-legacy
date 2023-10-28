namespace Bro.Toolbox.Logic.BehaviourTree
{
    public interface IRecheckUtility
    {
        bool NeedRecheck { get; }
        void Reset();
    }
}