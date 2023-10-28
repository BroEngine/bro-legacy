using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class ConversationActionTask : SubscribableTask<ConversationActionTask>
    {
        private readonly IClientContext _context;
        private readonly long _conversationId;
        private readonly int _userId;
        private readonly string _title;
        private readonly string _meta;
        private readonly ConversationAction _action;

        public ConversationActionTask(IClientContext context, long conversationId, int userId, ConversationAction action, string title = "", string meta = "")
        {
            _context = context;
            _conversationId = conversationId;
            _userId = userId;
            _title = title;
            _meta = meta;
            _action = action;
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<ConversationActionRequest>();
            request.ConversationId.Value = _conversationId;
            request.UserId.Value = _userId;
            request.Action.Value = (byte) _action;
            request.Title.Value = _title;
            request.Meta.Value = _meta;
            
            new SendRequestTask(request, _context.GetNetworkEngine()).Launch(taskContext);
            Complete();
        }
    }
}