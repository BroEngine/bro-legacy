using System;

namespace Bro
{
    public interface IScheduler
    {
        void Schedule<T>(Action<T> callback, T arg);

        void Schedule<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2);

        void Schedule<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3);

        void Schedule(Action callback);
        
        IDisposable ScheduleUpdate(Action handler, long interval);
        
        IDisposable Schedule(Action callback, long delayInMs);
    }
}