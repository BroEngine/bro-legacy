// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Threading;

namespace Bro.Threading
{
    public class ThreadPool : IThreadPool
    {
        private static bool _debugMode = false;

        private const int MaxActions = 1024 * 10;
        private const int WarningActions = MaxActions / 5;

        private readonly object[] _syncs;
        private readonly Action[][] _queue;
        private readonly int[] _queueCount;
        private readonly int _threadCount;
        private readonly BroThread[] _treads;
        private readonly ThreadParams[] _treadsParams;

        private readonly string _threadName;
        private Action _lastOperation;

        private readonly bool _canSubstitute;
        
        public event Action<IThreadPool> OnDispose;

        private class ThreadParams
        {
            public volatile bool Working;
            public volatile bool Terminated;
            public volatile bool PrimaryDequeued;
            public int Index;
        }

        public ThreadPool(string name, int threadCount, bool canSubstitute = false)
        {
            _threadName = name;
            _canSubstitute = canSubstitute;

            _threadCount = threadCount;
            _queueCount = new int[_threadCount];
            _syncs = new object[_threadCount];
            _treads = new BroThread[_threadCount];
            _treadsParams = new ThreadParams[_threadCount];
            _queue = new Action[_threadCount][];

            for (var i = 0; i < _threadCount; ++i)
            {
                _syncs[i] = new object();
                _queue[i] = new Action[MaxActions];

                StartThread(i);
            }
        }

        private void StartThread(int index)
        {
            var threadParam = new ThreadParams()
            {
                Index = index,
                Working = true
            };
            
            _treadsParams[index] = threadParam;
            _treads[index] = new BroThread(WorkCycle);
            _treads[index].Start(threadParam);

            if (_canSubstitute)
            {
                Bro.Log.Info("thread pool :: started new thread, index = " + index);
            }
        }

        

        public void AddOperation(int index, Action operation)
        {
            var threadIndex = index % _threadCount;
            lock (_syncs[threadIndex])
            {
                var queueCount = _queueCount[threadIndex];
                var substituteThread = false;

                if (_treadsParams[threadIndex].Terminated)
                {
                    return;
                }

                if (queueCount > WarningActions)
                {
                    if (_canSubstitute)
                    {
                        substituteThread = true;
                    }

                    Bro.Log.Info("thread pool :: " + _threadName + " index = " + threadIndex + ",  queue count = " + queueCount + ", action = " + operation.Method.Name + ", lastOperationName = " + _lastOperation?.Method.Name);
                }

                if (queueCount >= MaxActions)
                {
                    Bro.Log.Info("thread pool :: suspened " + _threadName + " index = " + threadIndex + ", queue count = " + queueCount + ", action = " + operation.Method.Name);
                    return;
                }

                if (substituteThread)
                {
                    if (_treadsParams[threadIndex].PrimaryDequeued)
                    {
                        _treadsParams[threadIndex].Working = false;
                        StartThread(threadIndex);
                    }
                }

                _queue[threadIndex][queueCount] = operation;
                _queueCount[threadIndex] = ++queueCount;
                
                Monitor.Pulse(_syncs[threadIndex]);
            }
        }

        private void WorkCycle(object parameter)
        {
            var config = parameter as ThreadParams;
            var passArray = new Action[MaxActions];
            var passCount = 0;
            var queueIndex = config.Index;
            var sync = _syncs[queueIndex];

            while (config.Working)
            {
                lock (sync)
                {
                    if (!config.Working)
                    {
                        Bro.Log.Info("thread pool :: thread interrupted, index = " + config.Index);
                        return;
                    }

                    var list = _queue[queueIndex];
                    passCount = _queueCount[queueIndex];
                    
                    _queue[queueIndex] = passArray;
                    _queueCount[queueIndex] = 0;
                    passArray = list;
                    config.PrimaryDequeued = true;

                    if (passCount == 0)
                    {
                        Monitor.Wait(sync, 30);
                    }
                }

                if (passCount > 0)
                {
                    for (int i = 0, max = passArray.Length; i < max; ++i)
                    {
                        var action = passArray[i];
                        if (action == null)
                        {
                            break;
                        }
                        
                        _lastOperation = action;
                        
                        if (_debugMode)
                        {
                            // ---- INVOKE DEBUG ---- //
                            action.Invoke();
                            // ---- INVOKE DEBUG---- //
                        }
                        else
                        {
                            // ---- INVOKE NORMAL---- //
                            try
                            {
                                //test
                                //var point = PerformanceMeter.Enabled ? PerformanceMeter.Register(Bro.PerformancePointType.ThreadPool, action.GetDescription()) : null;

                                action.Invoke();
                             
                                //test
                                //point?.Done();
                            }
                            catch (Exception e)
                            {
                                Bro.Log.Info("thread pool :: THREAD POOL FATAL ERROR, index = " + config.Index);
                                Bro.Log.Error(e);
                            }
                            // ---- INVOKE NORMAL---- //
                        }
                        

                        passArray[i] = null;

                        if (config.Terminated)
                        {
                            return;
                        }
                    }
                }
    
                if (config.Terminated)
                {
                    return;
                }

                if (!config.Working)
                {
                    Bro.Log.Info("thread pool :: thread interrupted, index = " + config.Index);
                    return;
                }  
            }
        }

        

        public void Stop()
        {
            Bro.Log.Info("thread pool :: " + _threadName + " terminated");
            _lastOperation = null;
            for (int i = 0; i < _threadCount; ++i)
            {
                _queueCount[i] = 0;
                _treadsParams[i].Working = false;
                _treadsParams[i].Terminated = true;
            }
            OnDispose?.Invoke(this);
        }
    }
}