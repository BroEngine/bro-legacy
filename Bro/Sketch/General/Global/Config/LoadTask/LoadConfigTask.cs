// ReSharper disable MergeConditionalExpression
// ReSharper disable ClassNeverInstantiated.Local

using Bro.Network;

namespace Bro.Sketch
{
    public class LoadConfigTask : SubscribableTask<LoadConfigTask>
    {
        public readonly ConfigDetails TargetConfigDetails;

        public IConfigStorage OutputStorage;

        private readonly RemoteConfigVersionStorage _remoteVersionStorage;
        private readonly LocalConfigVersionStorage _localVersionStorage;
        private readonly PersistentConfigVersionStorage _persistentVersionStorage;
        private readonly string _versionFileName;

        private readonly ConfigStorageCollector _collector;
        private readonly int _currentStorageVersion;
        private readonly bool _forceLocal;
        private readonly IWebContext _context;
        
        public LoadConfigTask(IWebContext context, ConfigDetails targetConfigDetails, VersionStorageContainer  versionStorageContainer, ConfigStorageCollector collector, bool forceLocal = false)
        {
            _context = context;
            TargetConfigDetails = targetConfigDetails;
            _collector = collector;
            _remoteVersionStorage = versionStorageContainer.RemoteConfigVersionStorage;
            _localVersionStorage = versionStorageContainer.LocalConfigVersionStorage;
            _persistentVersionStorage = versionStorageContainer.PersistentConfigVersionStorage;
            _versionFileName = versionStorageContainer.VersionFileName;

            var storage = _collector.GetStorage(targetConfigDetails);
            _currentStorageVersion = storage != null ? storage.Version : 0;
            _forceLocal = forceLocal;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            var remoteVersion = _remoteVersionStorage.GetVersion(TargetConfigDetails.RelativePath);
            var localVersion = _localVersionStorage != null ? _localVersionStorage.GetVersion(TargetConfigDetails.RelativePath) : -1;
            var persistentVersion = _persistentVersionStorage != null ? _persistentVersionStorage.GetVersion(TargetConfigDetails.RelativePath) : -1;
            if (_forceLocal)
            {
                localVersion = int.MaxValue;
            }

            var isRemoteVersionMax = remoteVersion > _currentStorageVersion && remoteVersion > localVersion && remoteVersion > persistentVersion;
            var isLocalVersionMax = localVersion > _currentStorageVersion && localVersion >= remoteVersion && localVersion > persistentVersion;
            var isPersistentVersionMax = persistentVersion > _currentStorageVersion && persistentVersion >= localVersion && persistentVersion >= remoteVersion;
            var isCurrentVersionMax = _currentStorageVersion>=remoteVersion && _currentStorageVersion>=localVersion && _currentStorageVersion>=persistentVersion;
            if (isCurrentVersionMax)
            {
                Complete();
                return;
            }

            BaseLoadConfigSubTask loadConfigTask = null;
            if (isRemoteVersionMax)
            {
                var arguments = new LoadRemoteConfigTask.Argument(_context, TargetConfigDetails)
                {
                    PersistentConfigVersionStorage = _persistentVersionStorage,
                    TargetVersion = remoteVersion,
                    NeedSavePersistent =  true,
                    VersionFileName = _versionFileName
                };    
                
                loadConfigTask = new LoadRemoteConfigTask(arguments);
            }
            else if (isLocalVersionMax)
            {
                loadConfigTask = new LoadLocalConfigTask(TargetConfigDetails, localVersion);
            }
            else if (isPersistentVersionMax)
            {
                loadConfigTask = new LoadPersistentConfigTask(TargetConfigDetails, persistentVersion);
            }

            if (loadConfigTask != null)
            {
                loadConfigTask.OnComplete += (t =>
                {
                    OutputStorage = t.OutputStorage;
                    Complete();
                });
                loadConfigTask.OnFail += (t => Fail());
                loadConfigTask.Launch(taskContext);
            }
        
            
        }
    }
}