namespace Bro.Sketch.Client
{
    public class CheckVersionTask : SubscribableTask<CheckVersionTask>
    {
        public bool HasMaintenance;
        public bool ActualVersion;
        public CheckVersionTask()
        {
            ActualVersion = true;
        }
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            Complete();
        }
        
    }
}