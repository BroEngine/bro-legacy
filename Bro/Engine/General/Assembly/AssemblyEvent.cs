using System;

namespace Bro
{
    public static class AssemblyEvent
    {
        public static event Action<Enum> Delegate;
        
        public static void Invoke(Enum e)
        {
            if (Delegate != null)
            {
                Delegate(e);
            }
        }
    }
}