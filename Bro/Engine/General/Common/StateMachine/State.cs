namespace Bro.StateMachine
{
    public interface IState
    {
        void OnStart();

        void OnFinish();

        void Update();
    }
}