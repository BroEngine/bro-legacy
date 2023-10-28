using System;

namespace Bro
{
    public static class AssemblyProcessExit
    {
        public static event Action<object, EventArgs> ProcessExit;
        
        static AssemblyProcessExit()
        {
            AppDomain.CurrentDomain.ProcessExit += OnApplicationExit;
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            ProcessExit?.Invoke(sender, e);
        }

        public static void InvokeProcessExit(object sender, EventArgs e)
        {
            OnApplicationExit(sender, e);
        }
    }
}