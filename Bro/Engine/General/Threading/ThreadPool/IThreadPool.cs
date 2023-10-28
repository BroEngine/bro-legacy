using System;

namespace Bro.Threading
{
    public interface IThreadPool
    {
        event Action<IThreadPool> OnDispose; 
        
        void AddOperation(int index, Action operation);
        void Stop();
    }
}