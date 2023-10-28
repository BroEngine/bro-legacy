using System;
using System.Collections.Generic;

namespace Bro.Threading
{
    public static class ThreadManagement
    {
        public delegate IThreadPool ThreadPoolCreatingDelegate(string name, int threadCount, bool canSubstitute = false);

        private static readonly List<IThreadPool> _registeredThreadPools = new List<IThreadPool>();
        private static ThreadPoolCreatingDelegate _customCreatingDelegate;

        static ThreadManagement()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        public static IThreadPool GetThreadPool(string name, int threadCount, bool canSubstitute = false)
        {
            IThreadPool result = null;
            if (_customCreatingDelegate != null)
            {
                result = _customCreatingDelegate(name, threadCount, canSubstitute);
            }
            else
            {
                result = new ThreadPool(name, threadCount, canSubstitute);
            }

            lock (_registeredThreadPools)
            {
                _registeredThreadPools.Add(result);
            }
            result.OnDispose += OnDisposeThreadPool;
            return result;
        }

        private static void OnDisposeThreadPool(IThreadPool pool)
        {
            lock (_registeredThreadPools)
            {
                _registeredThreadPools.FastRemove(pool);
            }
            pool.OnDispose -= OnDisposeThreadPool;
        }

        public static void RegisterCustomThreadPool(ThreadPoolCreatingDelegate customDelegate)
        {
            _customCreatingDelegate = customDelegate;
        }
        
        private static void OnProcessExit(object sender, EventArgs e)
        {
            TerminateAll();
        }

        public static void TerminateAll()
        {
            lock (_registeredThreadPools)
            {
                for (var i = 0; i < _registeredThreadPools.Count; ++i)
                {
                    _registeredThreadPools[i].Stop();
                }
                _registeredThreadPools.Clear();
            }
        }
    }
}