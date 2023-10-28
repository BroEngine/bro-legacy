using System;
using Sider;

namespace Bro.Service.Cache
{
    public class Operation : IOperation
    {
        public OperationType OperationType { get; }

        private readonly string _key;
        private readonly int _ttl;
        private string _json;
        private readonly Action<string> _callback;
        
        public Operation(string key, string value, int ttl)
        {
            OperationType = OperationType.Set;
            _ttl = ttl;
            _key = key;
            _json = value;
        }

        public Operation(string key, Action<string> callback)
        {
            OperationType = OperationType.Get;
            _key = key;
            _callback = callback;
        }
        
        void IOperation.Process(RedisClient redisClient)
        {
            switch (OperationType)
            {
                case OperationType.Set:
                    if (_ttl > 0)
                    {
                        redisClient.SetEX(_key, TimeSpan.FromSeconds(_ttl), _json);
                    }
                    else
                    {
                        redisClient.Set(_key, _json);
                    }
                    break;
                case OperationType.Get:
                    _json = redisClient.Get(_key);
                    break;
            }
        }

        void IOperation.InvokeCallback(bool result)
        {
            if (_callback != null)
            {
                if (result)
                {
                    _callback(_json);
                }
                else
                {
                    _callback(null);
                }
            }
        }       
    }
}