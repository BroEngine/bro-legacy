using System;
using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public class DictionaryParam<K, V> : BaseParam
        where K : BaseParam, new()
        where V : BaseParam, new()
    {
        private readonly IIntegerParam _paramPairsAmount;
        private readonly Dictionary<K, V> _params = new Dictionary<K, V>();

        public IEnumerable<KeyValuePair<K, V>> Params
        {
            get
            {
                CheckInitialized();
                return _params;
            }
        }

        public int Count
        {
            get
            {
                CheckInitialized();
                return _params.Count;
            }
        }
        public void Add(K key, V value)
        {
            if (key == null)
            {
                Bro.Log.Error("key is null");
                throw new ArgumentException("key is null");
            }

            if (!key.IsInitialized)
            {
                Bro.Log.Error("key is not initialized " + key.GetType() + " " + key.DebugData);
                throw new ArgumentException("key is not initialized " + key.GetType() + " " + key.DebugData);
            }

            if (value == null)
            {
                Bro.Log.Error("value is null");
                throw new ArgumentException("value is null");
            }

            if (!value.IsInitialized)
            {
                Bro.Log.Error("value is not initialized " + value.GetType() + " " + value.DebugData);
                throw new ArgumentException("value is not initialized " + value.GetType() + " " + value.DebugData);
            }

            _params.Add(key, value);
        }

        public DictionaryParam(int maxParamsAmount, bool isOptional = false) : base(isOptional)
        {
            if (maxParamsAmount <= byte.MaxValue)
            {
                _paramPairsAmount = new ByteParam();
            }
            else if (maxParamsAmount <= short.MaxValue)
            {
                _paramPairsAmount = new ShortParam();
            }
            else
            {
                _paramPairsAmount = new IntParam();
            }
        }

        public override bool IsInitialized
        {
            get
            {
                foreach (var p in _params)
                {
                    if (!p.Value.IsInitialized)
                    {
                        string dataString = (p.Value != null) ? p.Value.DebugData : "<null>";
                        Bro.Log.Error("DynamicTypedDictionaryParam: Param is not initialized " + p.GetType() +
                                      " data = " + dataString);
                        return false;
                    }

                    if (!p.Key.IsInitialized)
                    {
                        string dataString = (p.Key != null) ? p.Key.DebugData : "<null>";
                        Bro.Log.Error("DynamicTypedDictionaryParam: Param is not initialized " + p.GetType() +
                                      " data = " + dataString);
                        return false;
                    }
                }

                return true;
            }
        }

        public override void Write(IWriter writer)
        {
            _paramPairsAmount.Value = _params.Count;
            _paramPairsAmount.Write(writer);
            foreach (var p in _params)
            {
                p.Key.Write(writer);
                p.Value.Write(writer);
            }
        }

        public override void Read(IReader reader)
        {
            _paramPairsAmount.Read(reader);
            _params.Clear();
            var paramsAmount = _paramPairsAmount.Value;
            for (int i = 0; i < paramsAmount; ++i)
            {
                _params.Add(new K(), new V());
            }

            foreach (var p in _params)
            {
                p.Key.Read(reader);
                p.Value.Read(reader);
            }
        }

        public override void Cleanup()
        {
            foreach (var param in _params)
            {
                param.Key.Cleanup();

                param.Value.Cleanup();
            }
            
            _params.Clear();
            base.Cleanup();
        }

        //        public Dictionary<TK,TV> ToDictionary<TK,TV>(bool nullIfEmpty = false)
//        {
//            if (nullIfEmpty && _params.Count == 0)
//            {
//                return null;
//            }
//            var result = new Dictionary<TK,TV>();
//            foreach (var p in _params)
//            {
//                result.Add((TK) p.Key.Value, (TV) p.Value.Value);
//            }
//            return result;
//        }
    }
}