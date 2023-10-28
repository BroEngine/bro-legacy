namespace Bro
{
    public interface IPoolable
    {
        void OnPoolOut();
        void OnPoolIn();
    }
}