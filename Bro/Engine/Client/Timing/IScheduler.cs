using System;
using System.Collections;

namespace Bro.Client
{
    public partial class Timing
    {
        public interface IScheduler : IDisposable
        {
            IDisposable Schedule(Action callback, float delay = 0f);

            IDisposable ScheduleFixedUpdate(Handler handler);

            IDisposable ScheduleLateUpdate(Handler handler);

            IDisposable ScheduleUpdate(Handler handler);

            IDisposable ScheduleEase(Ease.Base ease);

            IDisposable StartCoroutine(IEnumerator enumerator);
        }
    }
}