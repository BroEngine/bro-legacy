using System;

namespace Bro.Threading
{
    public static class NetworkReadThreadPool
    {
        private static IThreadPool _pool;

        public static void ConfiguratePool(int count)
        {
            _pool = ThreadManagement.GetThreadPool("read_thread_pool", count,  true);
        }

        public static void AddOperation(int index, Action operation)
        {
            if (_pool == null)
            {
                throw new Exception("network read thread pool :: not configurated");
            }

            _pool.AddOperation(index, operation);
        }
    }
}