using System;
using System.Collections.Generic;
using Bro.Server.Context;
using Bro.Service;

namespace Bro.Sketch.Server
{
    public class UserStatusModule : IServerContextModule
    {
        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();

        protected IServerContext _context;
        
        public void Initialize(IServerContext context)
        {
            _context = context;
        }
        
        public void Register(Actor actor, string name, byte state, string data = "")
        {
            if (actor != null)
            {
                Log.Info($"user status :: status registered for user id = {actor.Profile.UserId} with state = {state}", 3);

                var request = CreateRequest(actor, name, state, data);
                _context.SendServiceRequest(request, null);
            }
            else
            {
                Log.Error("user status :: can not register null actor status");
            }
        }

        protected Infrastructure.UserStatusSetRequest CreateRequest(Actor actor, string name, byte state, string data = null)
        {
            if (actor == null || actor.Profile.UserId == 0)
            {
                return null;
            }
            
            var request = new Infrastructure.UserStatusSetRequest();
            
            var status = new UserStatus
            {
                UserId = actor.Profile.UserId,
                Name = name,
                SessionId = 0,
                Timestamp = TimeInfo.GlobalTimestamp,
                Channel = BrokerEngine.Instance.PrivateChannel.Path,
                State = state,
                Data = data
            };

            request.Status.Value = status;

            return request;
        }

        protected void GetUserStatus(int userId, Action<UserStatus> callback)
        {
            var request = new Infrastructure.UserStatusGetRequest();
            request.UserId.Value = userId;
            _context.SendServiceRequest(request, (response) =>
            {
                if (response != null)
                {
                    var getResponse = (Infrastructure.UserStatusGetResponse) response;
                    var status = getResponse.Status.Value;
                    callback?.Invoke(status);
                }
                else
                {
                    callback?.Invoke(null);
                }
            });
        }

        // [PeerJoinedHandler]
        // protected virtual void OnPeerJoin(IClientPeer peer, object data)
        // {
        //     var actor = peer.GetActor();
        //     Register(actor, 1);
        // }
        //
        // [PeerDisconnectedHandler]
        // protected virtual void OnPeerDisconnected(IClientPeer peer)
        // {
        //     var actor = peer.GetActor();
        //     Register(actor, 0);
        // }
    }
}