using System;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public abstract class CheatElement : MonoBehaviour, IDisposable
    {
        protected CheatModule Module;

        public void Setup(CheatModule module)
        {
            Module = module;
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}