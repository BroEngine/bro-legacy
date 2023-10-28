using System.Collections.Generic;

namespace Bro
{
    public class ConfigStorageCollector
    {
        private readonly Dictionary<ConfigDetails, IConfigStorage> _configStorages = new Dictionary<ConfigDetails, IConfigStorage>();
        
        private readonly object _configStoragesLock = new object();
        
        public IConfigStorage GetStorage(ConfigDetails details)
        {   
            if (details == null)
            {

                Bro.Log.Error("storage collector :: details is null");
                return null;
            }
            IConfigStorage result = null;
            lock (_configStoragesLock)
            {
                if (_configStorages.ContainsKey(details))
                {
                    result = _configStorages[details];
                }
            }
            return result;
        }
        
        public void MergeConfigStorage(ConfigDetails details, IConfigStorage configStorage)
        {
            lock (_configStoragesLock)
            {
                if (!_configStorages.ContainsKey(details))
                {
                    _configStorages[details] = configStorage;
                }
                else
                {
                    _configStorages[details].CopyFrom(configStorage);
                }
            }
        }
    }
}