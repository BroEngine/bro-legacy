using System;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network.TransmitProtocol
{
    public class ConfigDetailsParam : ParamsCollection
    {
        private readonly StringParam _localPath = new StringParam();
        private readonly StringParam _remotePath = new StringParam();
        private readonly TypeParam _storageType = new TypeParam();
        private readonly StringParam _versionKey = new StringParam();

        public ConfigDetailsParam(bool isOptional = false) : base(isOptional)
        {
            AddParam(_localPath);
            AddParam(_remotePath);
            AddParam(_storageType);
            AddParam(_versionKey);
        }

        public ConfigDetailsParam() : this(false)
        {
        }

        public ConfigDetails Value
        {
            get
            {
                if (IsInitialized)
                {
                    return new ConfigDetails(_versionKey.Value, _localPath.Value, _remotePath.Value, _storageType.Value);
                }
                else
                {
                    Log.Error("config details param :: param is not initialized");
                    return null;
                }
            }
            set
            {
                _localPath.Value = value.LocalPath;
                _remotePath.Value = value.RemotePath;
                _storageType.Value = value.StorageType;
                _versionKey.Value = value.RelativePath;
            }
        }
    }
}