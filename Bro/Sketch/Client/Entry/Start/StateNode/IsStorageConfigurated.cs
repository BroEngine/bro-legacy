using Bro.Core.Client;
using Bro.Toolbox;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class IsStorageInited<T>:Condition where T: class, IConfigStorage
    {

        public IsStorageInited()
        {
            ConditionHandler = CheckCondition;
        }

        bool CheckCondition()
        {
            return true;
            // var storage = ConfigStorageCollector.Instance.GetStorage<T>();
            // if (storage != null && storage.ConfigInited)
            // {
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }
        }
    }
}