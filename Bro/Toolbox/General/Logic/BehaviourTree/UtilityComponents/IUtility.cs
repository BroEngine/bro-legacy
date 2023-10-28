namespace Bro.Toolbox.Logic.BehaviourTree
{
    public interface IUtility
    {
        float UtilityValue { get; }
        string UtilityType { get; }
        void Reset();
    }
}