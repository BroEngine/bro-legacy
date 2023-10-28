using Bro.Client;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class CheckVersion : TaskRunner<CheckVersionTask>
    {
 
        public CheckVersion(IClientContext context) : base(context)
        {
        }
        
        protected override CheckVersionTask CreateTask()
        {
            var checkVersionTask = new CheckVersionTask();
            checkVersionTask.OnFail += (t =>
            {
                new LoadingInterruptionEvent(LoadingInterruptionType.ConnectionProblems).Launch();
            });
            checkVersionTask.OnComplete += (t =>
            {
                if (checkVersionTask.HasMaintenance)
                {
                    new LoadingInterruptionEvent(LoadingInterruptionType.Maintenance).Launch();
                } else if (!checkVersionTask.ActualVersion)
                {
                    new LoadingInterruptionEvent(LoadingInterruptionType.NeedNewVersion).Launch();
                }
            });
            return checkVersionTask;
        }
    }
}