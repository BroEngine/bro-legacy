using System;
using System.Collections.Generic;

namespace Bro.Sketch.Client
{
    public class ObjectFactoryStorage : IObjectFactoryStorage
    {
        private readonly Dictionary<int, IObjectFactory> _factories = new Dictionary<int, IObjectFactory>();
        private readonly Dictionary<IObjectBehaviour, IObjectFactory> _objects = new Dictionary<IObjectBehaviour, IObjectFactory>();
        
        public void AddFactory(int uid, IObjectFactory factory)
        {
            _factories[uid] = factory;
        }

        public bool HasFactory(int uid)
        {
            return _factories.ContainsKey(uid);
        }

        protected void CreateObject<T>(int uid, bool selfHandledDestroy, Action<T> completeCallBack)
        {
            CreateObject(uid, selfHandledDestroy, (result) =>
            {
                completeCallBack?.Invoke((T) result);
            });
        }

        public void CreateObject(int uid, bool selfHandledDestroy, Action<IObjectBehaviour> completeCallBack)
        {
            if (_factories.FastTryGetValue(uid, out var factory))
            {
                factory.CreateObject((behaviour) =>
                {
                    if (behaviour != null)
                    {
                        if (!selfHandledDestroy)
                        {
                            _objects[behaviour] = factory;
                        }
                    }
                    else
                    {
                        Bro.Log.Error("no IObjectBehaviour for resource = " + uid);
                    }
                    
                    completeCallBack?.Invoke(behaviour);
                });
            }
            else
            {
                Bro.Log.Error($"object factory component :: cannot find object factory for uid = {uid}, register it by calling AddFactory");
            }
        }
        
        public void DestroyObject(IObjectBehaviour behaviour)
        {
            if (_objects.FastTryGetValue(behaviour, out var factory))
            {
                _objects.Remove(behaviour);
                factory.DestroyObject(behaviour);
            }
            else
            {
                Bro.Log.Error($"object factory component :: cannot find object for = {behaviour}");
            }
        }

        public void Dispose()
        {
            foreach (var item in _factories)
            {
                item.Value.Dispose();
            }
            _factories.Clear();
            _objects.Clear();
        }
    }
}