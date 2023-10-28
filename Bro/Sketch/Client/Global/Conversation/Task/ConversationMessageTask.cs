using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class ConversationMessageTask : SubscribableTask<ConversationMessageTask>
    {
        private readonly IClientContext _context;
        private readonly long _conversationId;
        private readonly ConversationMessage _message;

        public ConversationMessageTask(IClientContext context, long conversationId, ConversationMessage message)
        {
            _context = context;
            _conversationId = conversationId;
            _message = message;
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<ConversationMessageRequest>();
            request.ConversationId.Value = _conversationId;
            request.Message.Value = _message;
            new SendRequestTask(request, _context.GetNetworkEngine()).Launch(taskContext);
            Complete();
        }
    }
}