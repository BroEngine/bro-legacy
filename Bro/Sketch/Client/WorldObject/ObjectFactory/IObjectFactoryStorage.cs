using System;

namespace Bro.Sketch.Client
{
    public interface IObjectFactoryStorage : IDisposable
    {
        void AddFactory(int uid, IObjectFactory factory);
        void CreateObject(int uid, bool selfHandledDestroy, Action<IObjectBehaviour> completeCallBack);
        void DestroyObject(IObjectBehaviour behaviour);
    }
}