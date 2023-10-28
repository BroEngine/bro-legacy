namespace Bro.Sketch
{
    public class LoadLocalConfigTask : BaseLoadConfigSubTask
    {
        public LoadLocalConfigTask(ConfigDetails details, int targetVersion) : base(details, targetVersion)
        {
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            var textAsset = (UnityEngine.TextAsset) UnityEngine.Resources.Load(Details.LocalPath);

            if (textAsset == null)
            {
                Bro.Log.Error("failed to load file = " + Details.LocalPath);
            }

            CreateOutputStorage(textAsset.text);
            Complete();
            #else
            throw new NotSupportedException("cannot use it on not unity side");
            #endif
        }
    }
}