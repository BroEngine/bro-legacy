namespace Bro.Toolbox.Node
{
    public interface IBehaviourNode
    {
        void OnEnter();
        void OnExit();
        void Update();
        void Reset();
    }
}