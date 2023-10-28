// using System;
// using System.Collections;
// using Bro.Client.Network;
// using Bro.Client.Network.Ping;
// using Bro.Client;
//
// namespace Bro.Sketch.Client
// {
//     public class PingManager : StaticSingleton<PingManager>
//     {
//         private IDisposable _updateCoroutine;
//         private readonly long _tryPeriod;
//         private readonly MedianStack<long> _pingResultStack;
//
//         public int Ping => (int)_pingResultStack.MedianValue;
//
//         public PingManager()
//         {
//             _pingResultStack = new MedianStack<long>(20);
//             _tryPeriod = GameConfig.Network.PingPeriod * 1000L;
//             NetworkEngine.Instance.OnSetStatus += OnSetNetworkStatus;
//         }
//
//         private void OnSetNetworkStatus(NetworkStatus status, int code)
//         {
//             switch (status)
//             {
//                 case NetworkStatus.Disconnected:
//                     StopUpdate();
//                     break;
//                 case NetworkStatus.Connected:
//                     StartUpdate();
//                     break;
//             }
//         }
//
//         private void PushPing(long ping)
//         {
//             _pingResultStack.AddSample(ping);
//         }
//
//         private IEnumerator UpdateCoroutine()
//         {
//             while (true)
//             {
//                 var pingTask = new PingTask();
//                 var waiter = new Timing.YieldWaitForTask<PingTask>(pingTask);
//                 yield return waiter;
//
//                 if (waiter.IsTaskCompleted)
//                 {
//                     PushPing(pingTask.PingResult);
//                 }
//
//                 yield return new Timing.YieldWaitForMilliseconds(_tryPeriod);
//             }
//         }
//
//         private void StartUpdate()
//         {
//             StopUpdate();
//             _updateCoroutine = GlobalContext.Instance.Scheduler.StartCoroutine(UpdateCoroutine());
//         }
//
//         private void StopUpdate()
//         {
//             if (_updateCoroutine != null)
//             {
//                 _updateCoroutine.Dispose();
//                 _updateCoroutine = null;
//             }
//         }
//
//     }
// }