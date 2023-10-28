#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT)

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bro.Toolbox.Client
{
    public class FixedGameObjectPool<T> : ObjectPool<T>, IDisposable where T : MonoBehaviour, IPoolable
    {
        public FixedGameObjectPool(int maxSize) : base(maxSize)
        {
            Disabler += o =>
            {
                o.gameObject.SetActive(false);
                o.OnPoolIn();
            };
            Enabler += o =>
            {
                o.gameObject.SetActive(true);
                o.OnPoolOut();
            };
            Destoyer += o =>
            {
                Object.Destroy(o.gameObject);
            };
            Constructor = () =>
            {
                Bro.Log.Error("objects in the fixed pool have run out, figure it out!");
                return null;
            };
        }

        public void Add(T o)
        {
            Release(o);
        }
        
        void IDisposable.Dispose()
        {
            Reset();
        }
    }
}

#endif