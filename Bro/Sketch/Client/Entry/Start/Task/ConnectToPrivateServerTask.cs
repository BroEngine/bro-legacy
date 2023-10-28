using Bro.Client;
using Bro.Client.Network;
using Bro.Network;

namespace Bro.Sketch.Client
{
    public class ConnectToPrivateServerTask : SubscribableTask<ConnectToPrivateServerTask>
    {
        private readonly IClientContext _context;
        private readonly NetworkEngine _networkEngine;
        private readonly string _ip;
        private readonly int _port;

        public ConnectToPrivateServerTask(IClientContext context, string ip, int port)
        {
            _context = context;
            _networkEngine = context.GetNetworkEngine();
            _ip = ip;
            _port = port;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
           
            if (_networkEngine.IsConnected())
            {
                _networkEngine.Disconnect(0);
            }

            var server = new ConnectionConfig(_ip, _port, ConnectionProtocol.Udp);
            
            var connectToServerTask = new EstablishConnectionTask(_context, server, _networkEngine);
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