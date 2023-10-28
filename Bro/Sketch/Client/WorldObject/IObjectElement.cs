namespace Bro.Sketch.Client
{
    public interface IObjectElement
    {
        void SetBehaviour(IObjectBehaviour behaviour);
        void Create();
        void Destroy();
    }
}