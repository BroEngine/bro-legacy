namespace Bro
{
    public interface ITaskContext
    {
        void Add(Task task);
        void Remove(Task task);
    }
}