using System;

namespace Bro.Threading
{
    public static class NetworkOperationThreadPool
    {
        private static IThreadPool _pool;

        public static void ConfiguratePool(int count)
        {
            _pool = ThreadManagement.GetThreadPool("operation_thread_pool", count);
        }

        public static void AddOperation(int index, Action operation)
        {
            if (_pool == null)
            {
                throw new Exception("network operation thread pool :: not configurated");
            }

            _pool.AddOperation(index, operation);
        }
    }
}