using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public abstract class ObjectElement : MonoBehaviour, IObjectElement
    {
        protected IClientContext Context  { get; private set; }
        protected IObjectBehaviour Behaviour  { get; private set; }

        public void SetBehaviour(IObjectBehaviour behaviour)
        {
            Context = behaviour.Context;
            Behaviour = behaviour;
        }
        
        public void Create()
        {
            OnObjectCreate();
        }
        
        public void Destroy()
        {
            OnObjectDestroy();
        }
        
        protected abstract void OnObjectCreate();

        protected abstract void OnObjectDestroy();
    }
}