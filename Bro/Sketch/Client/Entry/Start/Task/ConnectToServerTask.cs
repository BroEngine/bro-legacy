using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class ConnectToServerTask : SubscribableTask<ConnectToServerTask>
    {
        private readonly IClientContext _context;
        private readonly NetworkEngine _networkEngine;
        private readonly IServerListProvider _serverListProvider;


        public ConnectToServerTask(IClientContext context, IServerListProvider serverListProvider)
        {
            _context = context;
            _networkEngine = context.GetNetworkEngine();
            _serverListProvider = serverListProvider;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
           
            if (_networkEngine.IsConnected())
            {
                _networkEngine.Disconnect(0);
            }

            _serverListProvider.GetServerList((servers) =>
            {
                var chooseServerTask = new ChooseServerTask(_context, servers);
                chooseServerTask.OnFail += (OnFailChildTask);
                chooseServerTask.OnTerminate += (OnTerminateChildTask);
                
                chooseServerTask.OnComplete += (t2 =>
                {
                    var connectToServerTask = new EstablishConnectionTask(_context, chooseServerTask.ChosenServer, _networkEngine);
                    connectToServerTask.OnFail += (OnFailChildTask);
                    connectToServerTask.OnTerminate += (OnTerminateChildTask);
                    connectToServerTask.OnComplete += (t3 =>
                    {
                        var handshakeTask = new MakeHandShakeTask(_context, _networkEngine);
                        handshakeTask.OnComplete += (t4 => Complete());
                        handshakeTask.OnFail += (OnFailChildTask);
                        handshakeTask.OnTerminate += (OnFailChildTask);
                        handshakeTask.Launch(taskContext);
                    });
                    connectToServerTask.Launch(taskContext);
                });
                chooseServerTask.Launch(taskContext);
            });
        }

        private void OnFailChildTask(Task task)
        {
            Bro.Log.Error("connect to server task :: " + task.FailReason);
            Fail(task);
        }
        
        private void OnTerminateChildTask(Task task)
        {
            Terminate();
        }
    }
}