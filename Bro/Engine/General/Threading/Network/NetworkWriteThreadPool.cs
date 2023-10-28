using System;

namespace Bro.Threading
{
    public static class NetworkWriteThreadPool
    {
        private static IThreadPool _pool;

        public static void ConfiguratePool(int count)
        {
            _pool = ThreadManagement.GetThreadPool("write_thread_pool", count);
        }

        public static void AddOperation(int index, Action operation)
        {
            if (_pool == null)
            {
                throw new Exception("network write thread pool :: not configurated");
            }

            _pool.AddOperation(index, operation);
        }
    }
}