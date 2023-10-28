// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using Bro.Client;
//
// namespace Bro.Client.Network
// {
//     public class DelaySimulator
//     {
//         private class DelayedOperation
//         {
//             public float TimeLeft;
//             public Action Operation;
//         }
//
//         private readonly IDisposable _fixedUpdate;
//         private readonly List<DelayedOperation> _operations;
//         private readonly List<DelayedOperation> _toRemove;
//         private readonly bool _asStack;
//         private readonly Stopwatch _updateTimer;
//
//         public DelaySimulator(bool asStack)
//         {
//             _asStack = asStack;
//             _operations = new List<DelayedOperation>();
//             _toRemove = new List<DelayedOperation>();
//             _fixedUpdate = GlobalContext.Instance.Scheduler.ScheduleFixedUpdate(OnFixedUpdate);
//             _updateTimer = new Stopwatch();
//             _updateTimer.Start();
//         }
//
//         public void AddOperation(System.Action operation, float delay)
//         {
//             _operations.Add(new DelayedOperation()
//             {
//                 Operation = operation,
//                 TimeLeft = delay
//             });
//         }
//
//         private void Invoke(DelayedOperation operation)
//         {
//             if (operation.Operation != null)
//             {
//                 operation.Operation.Invoke();
//             }
//         }
//
//         private void OnFixedUpdate(float delta)
//         {
//             var t = _updateTimer.ElapsedMilliseconds / 1000.0f; 
//             _updateTimer.Reset();
//             _updateTimer.Start();
//             var ht = t / 2.0f;
//
//             for (var i = 0; i < _operations.Count; ++i)
//             {
//                 _operations[i].TimeLeft -= t;
//             }
//
//             if (_asStack)
//             {
//                 _toRemove.Clear();
//                 for (var i = 0; i < _operations.Count; ++i)
//                 {
//                     if (_operations[i].TimeLeft - ht <= 0.0f)
//                     {
//                         _toRemove.Add(_operations[i]);
//                     }
//                     else
//                     {
//                         break;
//                     }
//                 }
//
//                 for (var i = 0; i < _toRemove.Count; ++i)
//                 {
//                     Invoke(_toRemove[i]);
//                     _operations.Remove(_toRemove[i]);
//                 }
//             }
//             else
//             {
//                 for (var i = _operations.Count - 1; i > -1; i--)
//                 {
//                     if (_operations[i].TimeLeft - ht <= 0.0f)
//                     {
//                         Invoke(_operations[i]);
//                         _operations.RemoveAt(i);
//                     }
//                 }
//             }
//         }
//     }
// }