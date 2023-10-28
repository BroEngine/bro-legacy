using System;
using System.Collections.Generic;
using Sider;

namespace Bro.Service.Cache
{
    public class BatchOperation : IOperation
    {
        public OperationType OperationType { get { return OperationType.Get; } }

        private readonly List<string> _keys;
        private readonly List<string> _result;
        private readonly Action<List<string>> _callback;

        public BatchOperation(List<string> keys, Action<List<string>> callback )
        {
            _keys = keys;
            _callback = callback;
            _result = new List<string>();

            foreach (var key in _keys)
            {
                _result.Add(string.Empty);
            }
        }
  
        void IOperation.Process(RedisClient redisClient)
        {
            for (var i = 0; i < _keys.Count; ++i)
            {
                var key = _keys[i];
                var result = redisClient.Get(key);
                
                _result[i] = result;
            }
        }

        void IOperation.InvokeCallback(bool result)
        {
            if (_callback != null)
            {
                if (result)
                {
                    _callback(_result);
                }
                else
                {
                    _callback(null);
                }
            }
        }  
    }
}