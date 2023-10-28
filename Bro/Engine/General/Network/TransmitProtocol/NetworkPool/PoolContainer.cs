using System;
using System.Collections.Generic;

namespace Bro
{
    /// <summary>
    /// Base manager with object pools
    /// </summary>
    public sealed class PoolContainer<TValue> where TValue : class, IPoolElement
    {
        private readonly Dictionary<System.Type, ObjectPool<TValue>> _pools = new Dictionary<System.Type, ObjectPool<TValue>>();

        public readonly PoolContainerDebugger PoolContainerDebugger;

        private readonly object _locker = new object();
        private readonly int _defaultMaxPoolSize;
        
        private readonly  Dictionary<Type, int> _customMaxSizes = new Dictionary<Type, int>();

        public PoolContainer(bool isLogsEnabled, Dictionary<Type, int> customMaxSizes = null, int defaultMaxPoolSize = 64)
        {
            _defaultMaxPoolSize = defaultMaxPoolSize;
            if (customMaxSizes != null)
            {
                _customMaxSizes = customMaxSizes;
            }
            
            if (isLogsEnabled)
            {
                PoolContainerDebugger = new PoolContainerDebugger();
            }
        }

        public void SetPoolSize<T>(int size)
        {
            _customMaxSizes[typeof(T)] = size;
        }

        public T GetValue<T>() where T : class, TValue, new()
        {
            return GetValue<T>(CreateDelegate<T>);
        }
        
        TValue CreateDelegate<T>() where T : class, TValue, new()
        {
            var res = new T();
            res.IsPoolElement = true;
            PoolContainerDebugger?.LogNewPoolElement(res);
            return res;
        }
        
        public T GetValue<T>(Func<TValue> createDelegate) where T : class, TValue
        {
            var targetType = typeof(T);
            lock (_locker)
            {
                if (!_pools.ContainsKey(targetType))
                {
                    PoolContainerDebugger?.LogNewType(targetType);
                    int poolSize = _defaultMaxPoolSize;
                    if (_customMaxSizes.TryGetValue(targetType, out int customMaxPoolSize))
                    {
                        poolSize = customMaxPoolSize;
                    }

                    var newPool = new ObjectPool<TValue>(poolSize)
                    {
                        Constructor = createDelegate,
                    };

                    newPool.Destoyer += value =>
                    {
                        value.IsPoolElement = false;
                    };
                    
                    _pools.Add(targetType, newPool);
                }

                var ret = _pools[targetType].Acquire() as T;
                PoolContainerDebugger?.LogAcquire(ret);
                return ret;
            }
        }

        public void PutBack(TValue value)
        {
            if (!value.IsPoolElement)
            {
                if (value.IsPoolable)
                {
                    Log.Error($"[PoolContainer] Object type not spawned through pool: {value.GetType()}");
                }
                return;
            }
            
            var targetType = value.GetType();
            lock (_locker)
            {
                if (!_pools[targetType].Release(value))
                {
                    Log.Error($"[PoolContainer] Object will be destroyed: {targetType} {_pools[targetType].ObjectsAmount} / {_pools[targetType].MaxSize}");
                }
            }
            PoolContainerDebugger?.LogReturn(value);
        }

        public void Reset()
        {
            lock (_locker)
            {
                Log.Info("[PoolContainer] Clear pool");
                
                foreach (var pool in _pools)
                {
                    pool.Value.Reset();
                }
            }
        }
    }
}
