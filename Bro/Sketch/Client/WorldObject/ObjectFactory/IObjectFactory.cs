using System;
using System.Collections;

namespace Bro.Sketch.Client
{
    /* один ресурс -> одна фабрика; именно ресурс, не тип ресурса */
    public interface IObjectFactory : IDisposable
    {
        void CreateObject(Action<IObjectBehaviour> completeCallback); 
        void DestroyObject(IObjectBehaviour objectBehaviour);
        IEnumerator Initialize();
    }
}