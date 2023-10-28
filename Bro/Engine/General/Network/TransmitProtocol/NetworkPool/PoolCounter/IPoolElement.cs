namespace Bro
{
    public interface IPoolElement
    {
        bool IsPoolElement { get; set; }
        bool IsPoolable { get; }
    }
}