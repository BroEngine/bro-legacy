using System;

namespace Bro.Threading
{
    public interface IThreadScheduler : IDisposable
    {
        IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs);
        IDisposable ScheduleOnInterval(Action<int> action, long firstInMs, long regularInMs);
    }
}