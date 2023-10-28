using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class FriendsSetTask : SubscribableTask<FriendsSetTask>
    {
        private readonly IClientContext _context;
        private readonly int _userId;
        private readonly FriendState _state;

        public FriendsSetTask(IClientContext context, int userId, FriendState state)
        {
            _context = context;
            _userId = userId;
            _state = state;
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<FriendsSetRequest>();
            request.UserId.Value = _userId;
            request.State.Value = (byte) _state;
            
            var task = new SendRequestTask<FriendsSetResponse>(_context, request, _context.GetNetworkEngine());
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