using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class FriendsDeleteTask : SubscribableTask<FriendsDeleteTask>
    {
        private readonly IClientContext _context;
        private readonly int _userId;

        public FriendsDeleteTask(IClientContext context, int userId)
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
            var request = NetworkPool.GetOperation<FriendsDeleteRequest>();
            request.UserId.Value = _userId;
            
            var task = new SendRequestTask<FriendsDeleteResponse>(_context, request, _context.GetNetworkEngine());
            task.OnComplete += (t =>
            {
                var response = t.Response;
                if (response.Result.Value == 1)
                {
                    Complete();
                }
                else
                {
                    Fail();
                }
            });
            task.OnFail += (t => Fail());
            task.Launch(_context);
        }
    }
}