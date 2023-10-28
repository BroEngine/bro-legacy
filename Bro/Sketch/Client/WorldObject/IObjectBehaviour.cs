using System;
using System.Collections.Generic;
using Bro.Client;

namespace Bro.Sketch.Client
{
    public interface IObjectBehaviour : IPoolable
    {
        public IClientContext Context { get; }
        
        void Init(IClientContext context);
        void Create();
        void Destroy();
        
        
        event Action<float> OnUpdate;
        event Action<float> OnFixedUpdate;
        
        T GetElement<T>() where T : IObjectElement;
        IObjectElement[] GetElements();
    }
}