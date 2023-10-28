using System;
using System.Collections.Generic;
using System.Text;

namespace Bro
{
    public class PoolContainerDebugger
    {
        List<string> _typesFilter = new List<string>()
        {
            //"Bro.Sketch.Network.GameInfoEvent",
        };

        bool IsSkippeByFilter(System.Type type)
        {
            return (_typesFilter.Count != 0 && !_typesFilter.Contains(type.FullName));
        }
        
        public void LogNewType(System.Type type)
        {
            if(IsSkippeByFilter(type)) return;
            
            Log.Error($"[PoolManagerDebugger] New pool {type}");
        }
        
        public void LogNewPoolElement(object obj)
        {
            if(IsSkippeByFilter(obj.GetType())) return;
            
            Log.Info($"[PoolManagerDebugger] Adding new element {obj.GetType()} {obj.GetHashCode()}");
        }
        
        public void LogAcquire(object obj)
        {
            if(IsSkippeByFilter(obj.GetType())) return;

            Log.Error($"[PoolManagerDebugger] Acquire {obj.GetType()} {obj.GetHashCode()}");
        }
        
        public void LogReturn(object obj)
        {
            if(IsSkippeByFilter(obj.GetType())) return;
            
            Log.Info($"[PoolManagerDebugger] Release {obj.GetType()} {obj.GetHashCode()}");
        }
    }
}