using System;

namespace Bro.Client
{
    public partial class Timing
    {
        public delegate void Handler(float deltaTime);
        
        internal abstract class Base : IDisposable
        {
            public Handler TimingHandler;

            public abstract void Stop();

            void IDisposable.Dispose()
            {
                Stop();
            }
        }
    }
    
}