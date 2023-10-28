using System;

namespace Bro.Sketch
{
    public class LoadPersistentConfigTask : BaseLoadConfigSubTask
    {
        private readonly bool _isSourceOptional;
        public LoadPersistentConfigTask(ConfigDetails details, int targetVersion, bool isSourceOptional = false) : base(details, targetVersion)
        {
            _isSourceOptional = isSourceOptional;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            var persistentData = PersistentStorage.LoadText(Details.RelativePath);
            if (!string.IsNullOrEmpty(persistentData))
            {
                CreateOutputStorage(persistentData);
            }
            else if (_isSourceOptional)
            {
                Bro.Log.Info("load persistent config task :: current storage type " + Details.StorageType + " and still empty");
            }
            else
            {
                Bro.Log.Error("load persistent config task :: current source is empty " + Details.StorageType);
            }

            Complete();
#else
            throw new NotSupportedException("cannot use it on not unity side");
#endif
        }
    }
}