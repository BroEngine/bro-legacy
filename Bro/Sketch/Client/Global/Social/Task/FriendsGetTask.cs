using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class FriendsGetTask : SubscribableTask<FriendsGetTask>
    {
        private readonly IClientContext _context;
        private bool _isServiceAvailable;
        private List<Friend> _friends = new List<Friend>();
        
        public List<Friend> Friends => _friends;

        public FriendsGetTask(IClientContext context)
        {
            _context = context;
        }
        
        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<FriendsGetRequest>();
            
            var task = new SendRequestTask<FriendsGetResponse>(_context, request, _context.GetNetworkEngine());
            task.OnComplete += (t =>
            {
                var response = t.Response;
                _isServiceAvailable = response.IsServiceAvailable.Value;

                if (!_isServiceAvailable)
                {
                    Fail();
                }
                else
                {
                    if (response.Friends.IsInitialized)
                    {
                        foreach (var friendParam in response.Friends.Params)
                        {
                            var friend = friendParam.Value;
                           
                            _friends.Add(friend);
                        }
                    }
                    
                    Complete();
                }
            });
            task.OnFail += (t => Fail());
            task.Launch(_context);
        }
    }
}