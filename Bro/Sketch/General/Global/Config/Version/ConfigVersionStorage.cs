using System;
using System.Collections.Generic;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch
{
    public abstract class ConfigVersionStorage : Bro.SyncSingleConfigStorage<Dictionary<string,VersionModel>>
    {
        public int GetVersion(string fileName)
        {
            var config = GetConfig();
            var version = 0;
            if (config == null)
            {
                Bro.Log.Error($"config version storage {GetType()} :: is not initialized");
            }
            else if (config.TryGetValue(fileName, out var versionModel))
            {
                version = versionModel.Version;
            }
            else
            {
                Bro.Log.Error($"config version storage {GetType()} :: cannot find version for {fileName}");
            }
            return version;
        }


        // public bool IsOnlyOne<T>()
        // {
        //     var result = true;
        //     var type = typeof(T);
        //     var entity = ConfigStorageCollection.Instance.ConfigDetailsCollector.GetConfigDetails(type);
        //     var configName = entity.File;
        //     var config = GetConfig();
        //     if (config != null && config.TryGetValue(configName, out var versionModel))
        //     {
        //         result = versionModel.IsOnlyOne;
        //     }
        //     return result;
        // }

        public bool HasSeparateConfig(string name)
        {
            bool result = false;
            var config = GetConfig();
            if (config != null && config.TryGetValue(name, out var versionModel))
            {
                result = versionModel.HasSeparateConfig;
            }

            return result;
        }
        
    }
    
    [Serializable, TypeBinder("remote_config_version_storage")]
    public class RemoteConfigVersionStorage : ConfigVersionStorage
    {
        
    }
    
    [Serializable, TypeBinder("local_config_version_storage")]
    public class LocalConfigVersionStorage : ConfigVersionStorage
    {
        
    }
    [Serializable, TypeBinder("persistent_config_version_storage")]
    public class PersistentConfigVersionStorage : ConfigVersionStorage
    {
        
    }
}