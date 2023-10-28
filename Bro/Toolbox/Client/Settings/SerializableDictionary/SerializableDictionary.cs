using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {
        [NonSerialized] private Dictionary<TKey, TValue> _dict;
        [NonSerialized] private IEqualityComparer<TKey> _comparer;
        
        [SerializeField] private TKey[] _keys;
        [SerializeField] private TValue[] _values;
        
        public SerializableDictionary()
        {

        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
        }
        
        public IEqualityComparer<TKey> Comparer
        {
            get { return _comparer; }
        }

        public int Count
        {
            get { return _dict?.Count ?? 0; }
        }

        public void Add(TKey key, TValue value)
        {
            CreateIfNull();
            _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict != null && _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                CreateIfNull();
                return _dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            return _dict != null && _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict != null)
            {
                return _dict.TryGetValue(key, out value);
            }
            value = default;
            return false;
        }

        public ICollection<TValue> Values
        {
            get
            {
                CreateIfNull();
                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dict == null)
                {
                    throw new KeyNotFoundException();
                }
                return _dict[key];
            }
            set
            {
                CreateIfNull();
                _dict[key] = value;
            }
        }

        public void Clear()
        {
            if (_dict != null)
            {
                _dict.Clear();
            }
        }

        private void CreateIfNull()
        {
            if (_dict == null)
            {
                _dict = new Dictionary<TKey, TValue>(_comparer);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            CreateIfNull();
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dict != null && (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dict == null)
            {
                return;
            }
            
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dict != null && (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _dict?.GetEnumerator() ?? default(Dictionary<TKey, TValue>.Enumerator);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dict?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(_keys != null && _values != null)
            {
                CreateIfNull(); 
                _dict.Clear();
                for(var i = 0; i < _keys.Length; i++)
                {
                    if (i < _values.Length)
                    {
                        _dict[_keys[i]] = _values[i];
                    }
                    else
                    {
                        _dict[_keys[i]] = default;
                    }
                }
            }

            _keys = null;
            _values = null;
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if(_dict == null || _dict.Count == 0)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                var count = _dict.Count;
                _keys = new TKey[count];
                _values = new TValue[count];
                var i = 0;
                var e = _dict.GetEnumerator();
                while (e.MoveNext())
                {
                    _keys[i] = e.Current.Key;
                    _values[i] = e.Current.Value;
                    i++;
                }
                e.Dispose();
            }
        }
    }
}