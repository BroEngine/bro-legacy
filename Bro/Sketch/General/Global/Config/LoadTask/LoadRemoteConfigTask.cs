using Bro.Network;
using Bro.Network.Web;

namespace Bro.Sketch
{
    public class LoadRemoteConfigTask : BaseLoadConfigSubTask
    {
        public class Argument
        {
            public readonly IWebContext Context;
            public readonly ConfigDetails Details;
            
            public PersistentConfigVersionStorage PersistentConfigVersionStorage = null;
            public int TargetVersion = 0;
            public bool NeedSavePersistent = false;
            public string VersionFileName = null;
            
            public Argument(IWebContext context, ConfigDetails details)
            {
                Context = context;
                Details = details;
            }
        }

        private readonly Argument _arguments;
       
        public LoadRemoteConfigTask(Argument argument) : base(argument.Details,argument.TargetVersion)
        {
            _arguments = argument;
            
            #if ! (UNITY_IOS && UNITY_ANDROID && UNITY_STANDALONE && UNITY_XBOXONE && UNITY_PS4 && UNITY_WEBGL && UNITY_WII)
            _arguments.NeedSavePersistent = false;
            #endif
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            
            var request = new RegularWebRequestTask(_arguments.Context, RegularWebRequestTask.RequestType.Get, Details.RemotePath);
            request.OnComplete += (task => ConfigResultProcess(request));
            request.OnFail += (task => Fail("load remote config task :: failed"));
            request.Launch(taskContext);
        }
        
        private void ConfigResultProcess(RegularWebRequestTask request)
        {
            var json = request.Response;
            CreateOutputStorage(json);
            if (_arguments.NeedSavePersistent)
            {
                var versionConfig = _arguments.PersistentConfigVersionStorage.GetConfig();
                versionConfig[Details.Name].Version = _arguments.TargetVersion;
                var modifiedJson = _arguments.PersistentConfigVersionStorage.Dump();
                var relativePath = _arguments.VersionFileName;
                PersistentStorage.SaveText(relativePath, modifiedJson);
            }
            Complete();
        }
    }
}