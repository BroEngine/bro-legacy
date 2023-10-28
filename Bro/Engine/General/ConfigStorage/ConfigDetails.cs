using System;
using System.IO;
using Bro.Json;
using Bro.Network.Tcp.Engine.Client;

namespace Bro
{
    [Serializable]
    public class ConfigDetails
    {
        [JsonIgnore] public string Name => GetName();
        [JsonProperty("local_path")] public string LocalPath;
        [JsonProperty("remote_path")] public string RemotePath;
        [JsonProperty("type")] public Type StorageType;
        [JsonProperty("relative_path")] public string RelativePath;

        public ConfigDetails(string relativePath, string localPath, string remotePath, Type storageType)
        {
            LocalPath = localPath;
            RemotePath = remotePath;
            StorageType = storageType;
            RelativePath = relativePath;
        }

        private string GetName()
        {
            var separator = '.';
            var path = RelativePath;
            var file = Path.GetFileName(path);
            var idx = file.IndexOf(separator);
            return idx > 0 ? file.Substring(0, idx).Trim() : null;
        }

        
        public override bool Equals(object obj)
        {
            if (obj is ConfigDetails inputDetails)
            {
                var isEqualType = StorageType == inputDetails.StorageType;
                var isEqualLocalPath = LocalPath == inputDetails.LocalPath;
                return isEqualType && isEqualLocalPath;
            }
            else
            {
                Bro.Log.Error("config details equals :: null object");
                return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((LocalPath != null ? LocalPath.GetHashCode() : 0) * 397) ^ (StorageType != null ? StorageType.GetHashCode() : 0);
            }
        }
    }
}