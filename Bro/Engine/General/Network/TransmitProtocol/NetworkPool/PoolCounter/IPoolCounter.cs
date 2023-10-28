namespace Bro
{
    public interface IPoolCounter 
    {
        void Retain();
        void Release();
    }
}