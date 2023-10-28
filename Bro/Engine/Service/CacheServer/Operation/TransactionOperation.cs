using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sider;

namespace Bro.Service.Cache
{
    public class TransactionOperation : IOperation
    {
        public OperationType OperationType { get; }
        
        private readonly string _key;
        private readonly int _ttl;
        private readonly int _attempts;

        private bool _transactionResult;

        private readonly Action<bool> _callback;
        private readonly CacheServerDelegate.ValueJsonCheck _jsonCheckCallback;
        private readonly CacheServerDelegate.ValueJsonCreate _creationCallback;
        
        public TransactionOperation( string key, int ttl, int attempts, CacheServerDelegate.ValueJsonCheck checker, CacheServerDelegate.ValueJsonCreate creator, Action<bool> callback )
        {
            _key = key;
            _ttl = ttl;
            _attempts = attempts;
            _jsonCheckCallback = checker;
            _creationCallback = creator;
            _callback = callback;
        }

        private bool Itterate(RedisClient redisClient) /* false if transaction failed */
        {
            redisClient.Watch(_key);
            
            var read = redisClient.Get(_key);
            var cheched = _jsonCheckCallback != null ? _jsonCheckCallback.Invoke(read) : true;
            if (cheched)
            {
                var json = _creationCallback(read);
                if (json == null)
                {
                    redisClient.Del(_key);
                    return true;
                }

                redisClient.Multi();
                if (_ttl > 0)
                {
                    redisClient.SetEX(_key, TimeSpan.FromSeconds(_ttl), json);
                }
                else
                {
                    redisClient.Set(_key, json);
                }

                try
                {
                    var results = redisClient.Exec();
                    var result = IsBatchResultSuccessfull(results);

                    if (result)
                    {
                        _transactionResult = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Bro.Log.Info("redis :: transaction failed, " + e.Message);
                    return false;
                }
            }
            
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBatchResultSuccessfull( IEnumerable<object> results )
        {
            var successfull = false;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!(bool)result)
                    {
                        return false;
                    }
                    successfull = true;
                }
            }
            return successfull;
        }

        public void Process(RedisClient redisClient)
        {
            for (var i = 0; i < _attempts; ++i)
            {
                var transactionCompleted = Itterate( redisClient );
                if (transactionCompleted)
                {
                    break;
                }
            }
        }

        public void InvokeCallback(bool result)
        {
            if (_callback != null)
            {
                if (result)
                {
                    _callback(_transactionResult);
                }
                else
                {
                    _callback(false);
                }
            }
        }
    }
}