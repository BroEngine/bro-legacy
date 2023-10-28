using Bro.Client;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class ConnectToServer : TaskRunner<ConnectToServerTask>
    {
        private readonly IClientContext _context;
        private readonly IServerListProvider _serverListProvider;

        public ConnectToServer(IClientContext context, IServerListProvider serverListProvider) : base(context)
        {
            _context = context;
            _serverListProvider = serverListProvider;
            Bro.Log.Assert(_context != null);
        }

        protected override ConnectToServerTask CreateTask()
        {
            var connectTask = new ConnectToServerTask(_context, _serverListProvider);
            connectTask.OnFail += (t =>
            {
                new LoadingInterruptionEvent(LoadingInterruptionType.ConnectionProblems).Launch();
            });
            return connectTask;
        }
    }
}