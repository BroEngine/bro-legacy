using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class ConversationGetTask : SubscribableTask<ConversationGetTask>
    {
        private readonly IClientContext _context;
        private readonly int _userId;
        
        public ConversationGetTask(IClientContext context, int userId)
        {
            _context = context;
            _userId = userId;
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<Network.ConversationGetRequest>();
            request.UserId.Value = _userId;
            new SendRequestTask(request, _context.GetNetworkEngine()).Launch(taskContext);
            Complete();
        }
    }
}