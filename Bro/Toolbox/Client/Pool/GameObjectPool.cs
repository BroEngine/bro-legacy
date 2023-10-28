#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT)

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bro.Toolbox.Client
{
    public class GameObjectPool<T> : ObjectPool<T>, IDisposable where T : MonoBehaviour, IPoolable
    {
        private Transform _parent;
        
        public GameObjectPool(GameObject prefab, int maxSize) : base(maxSize)
        {
            if (prefab == null)
            {
                Bro.Log.Error("pool: prefab is null");
            }

            var prefab1 = prefab;
            
            Disabler += o =>
            {
                o.gameObject.SetActive(false);
                o.OnPoolIn();
            };
            Enabler += o =>
            {
                o.gameObject.SetActive(true);
                o.gameObject.transform.localPosition = prefab1.transform.localPosition;
                o.gameObject.transform.localRotation = prefab1.transform.localRotation;
                o.OnPoolOut();
            };
            Destoyer += o =>
            {
                Object.Destroy(o.gameObject);
            };
            Constructor = () =>
            {
                var o = Object.Instantiate(prefab1, Vector3.zero, Quaternion.identity);
                if (o == null)
                {
                    Bro.Log.Error("can not create object from prefab = " + prefab1);
                }

                var c = o.GetComponent<T>();

                if (c == null)
                {
                    Bro.Log.Error("no script " + typeof(T) + " on object = " + o.name);
                }
                
                if (_parent != null)
                {
                    o.transform.SetParent(_parent, false);
                }
                return c;
            };
        }

        public GameObjectPool(GameObject prefab, Transform parent, int maxSize) : this(prefab, maxSize)
        {
            _parent = parent;
        }

        public void Preload(int objectsCount)
        {
            if (objectsCount <= 0)
            {
                return;
            }
            var objects = new T[objectsCount];
            for (var i = 0; i < objectsCount; ++i)
            {
                objects[i] = Acquire();
            }
            
            for (int i = 0; i < objectsCount; ++i)
            {
                Release(objects[i]);
            }
        }

        public void SetParentForPoolObjects(Transform parent)
        {
            _parent = parent;
            for (var i = 0; i < Objects.Length; i++)
            {
                Objects[i].transform.SetParent(parent,false);
            }
        }

        void IDisposable.Dispose()
        {
            Reset();
        }
    }
}

#endif