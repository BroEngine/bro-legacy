#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using System;
using System.Collections.Generic;
using Bro.Client;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [Serializable]
    public class UiObjectsPool<T> where T : MonoBehaviour, IPoolable
    {
        private GameObjectPool<T> _gameObjectPool;
        private readonly IClientContext _context;
        
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _parent;

        List<T> _acquiredElems = new List<T>();

        public bool IsInitialized { get; private set; }
        protected UiObjectsPool() {}
        
        public UiObjectsPool(IClientContext context, GameObject prefab, Transform parent, int count = 100)
        {
            _context = context;
            _prefab = prefab;
            _parent = parent;
            InitPool(count);
        }

        public void InitPool(int count = 100)
        {
            _gameObjectPool = new GameObjectPool<T>(_prefab, _parent, count);
            IsInitialized = true;
        }

        public T Acquire()
        {
            var retElem = _gameObjectPool.Acquire();
            retElem.transform.SetSiblingIndex(_acquiredElems.Count);
            _acquiredElems.Add(retElem);
            return retElem;
        }

        public void Release(T obj)
        {
            var index = _acquiredElems.IndexOf(obj);

            if (index != -1)
            {
                _gameObjectPool.Release(obj);
                _acquiredElems.Remove(obj);
            }
            else
            {
                Log.Error("Object was not spawned true pool");
            }
        }

        public void ReleaseAllAcquired()
        {
            foreach (var elem in _acquiredElems)
            {
                _gameObjectPool.Release(elem);
            }   
            
            _acquiredElems.Clear();
        }
    }
}
#endif