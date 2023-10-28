using System;
using System.Reflection;

namespace Bro.Sketch
{
    public class BaseLoadConfigSubTask : SubscribableTask<BaseLoadConfigSubTask>
    {
        private readonly int _targetVersion;
        public IConfigStorage OutputStorage { get; private set; }
        public readonly ConfigDetails Details;

        protected BaseLoadConfigSubTask(ConfigDetails details, int targetVersion)
        {
            Details = details;
            _targetVersion = targetVersion;
        }
        
        protected void CreateOutputStorage(string configData)
        {
            OutputStorage = CreateStorage(Details);
            OutputStorage.Initialize(configData, _targetVersion);
        }
        
        private IConfigStorage CreateStorage(ConfigDetails details)
        {
            var constructorInfo = details.StorageType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                Bro.Log.Error("base load config sub task :: null constructor info");
                return null;
            }
            try
            {
                var result = (IConfigStorage)constructorInfo.Invoke(new object[] { });
                return result;
            }
            catch (Exception e)
            {
                Bro.Log.Error($"base load config sub task :: storage type = {details.StorageType} exception = {e}");
                throw;
            }
        }

    }
}