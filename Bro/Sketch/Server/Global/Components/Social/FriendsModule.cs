using System.Collections.Generic;
using Bro.Network;
using Bro.Network.Service;
using Bro.Server.Context;
using Bro.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class FriendsModule : IServerContextModule
    {
        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();

        private IServerContext _context;
        
        public void Initialize(IServerContext context)
        {
            _context = context;
        }
        
        [RequestHandler(Request.OperationCode.Social.FriendsGet)]
        private INetworkResponse OnFriendsGetRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            var actor = peer.GetActor();
            var userId = actor.Profile.UserId;
            
            Log.Info("friend component :: get friend request received from user id = " + userId);
            
            var response = (FriendsGetResponse) NetworkOperationFactory.CreateResponse(request);
            response.IsServiceAvailable.Value = false;
            response.IsHeld = true;
            
            var serviceRequest = new Infrastructure.FriendsGetRequest();
            serviceRequest.UserId.Value = userId;
            
            _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
            {
                if (serviceResponse != null)
                {
                    var serviceFriendsResponse = (Infrastructure.FriendsGetResponse) serviceResponse;
                    if (serviceFriendsResponse.Friends.IsInitialized)
                    {
                        foreach (var friendParam in serviceFriendsResponse.Friends.Params)
                        {
                            var friend = friendParam.Value;
                            var param = NetworkPool.GetParam<FriendParam>();
                            param.Value = friend;
                            response.Friends.Add(param);
                        }
                    }
                    
                    response.IsServiceAvailable.Value = true;
                }
                else
                {
                    Log.Info("friend component :: get friend request failed for user id = " + userId);
                }

                response.IsHeld = false;
            });
                
            return response;
        }
        
        [RequestHandler(Request.OperationCode.Social.FriendsSet)]
        private INetworkResponse OnFriendsSetRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            var actor = peer.GetActor();
            var authorUserId = actor.Profile.UserId;
            var friendsSetRequest = (FriendsSetRequest) request;
            var friendUserId = friendsSetRequest.UserId.Value;
            var status = friendsSetRequest.State.Value;
            
            var response = (FriendsSetResponse) NetworkOperationFactory.CreateResponse(request);
            response.Result.Value = 0;
            response.IsHeld = true;
            
            var serviceRequest = new Infrastructure.FriendsSetRequest();
            serviceRequest.State.Value = status;
            serviceRequest.UserId.Value = friendUserId;
            serviceRequest.AuthorUserId.Value = authorUserId;
            
            _context.SendServiceRequest( serviceRequest, (serviceResponse) =>
            {
                if (serviceResponse != null)
                {
                    var serviceFriendsResponse = (Infrastructure.FriendsSetResponse) serviceResponse;
                    response.Result.Value = serviceFriendsResponse.Result.Value;
                }

                response.IsHeld = false;
            });
            
            return response;
        }
        
        [RequestHandler(Request.OperationCode.Social.FriendsDelete)]
        private INetworkResponse OnFriendDeleteRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            var actor = peer.GetActor();
            var friendsDeleteRequest = (FriendsDeleteRequest) request;
            var user01 = actor.Profile.UserId;
            var user02 = friendsDeleteRequest.UserId.Value;
            
            var response = (FriendsDeleteResponse) NetworkOperationFactory.CreateResponse(request);
            response.IsHeld = true;
            response.Result.Value = 0;
            
            var serviceRequest = new Infrastructure.FriendsDeleteRequest();
            
            serviceRequest.UserId01.Value = user01;
            serviceRequest.UserId02.Value = user02;
           
            _context.SendServiceRequest( serviceRequest, (serviceResponse) =>
            {
                if (serviceResponse != null)
                {
                    var deleteResponse = (Infrastructure.FriendsDeleteResponse) serviceResponse;
                    var result = deleteResponse.Result.Value;
                    response.Result.Value = result;
                }

                response.IsHeld = false;
            });
            
            return response;
        }  
        
        [RequestHandler(Request.OperationCode.Social.FriendsIdentity)]
        private INetworkResponse OnFriendIdentityRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            var actor = peer.GetActor();
            var userId = actor.Profile.UserId;
            var friendsIdentifyRequest = (FriendsIdentityRequest) request;
            
            var response = (FriendsIdentityResponse) NetworkOperationFactory.CreateResponse(request);
            response.IsHeld = true;
            
            var serviceRequest = new Infrastructure.FriendsIdentityRequest();
            
            var identityParams = friendsIdentifyRequest.Friends.Params;
            foreach (var identityParam in identityParams)
            {
                serviceRequest.Friends.Params.Add( identityParam.Clone() );
            }
            
            _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
            {
                Log.Info("friend component :: identify friends service response received with result = " + ( serviceResponse != null ) + " for user id = " + userId);

                if (serviceResponse != null)
                {
                    var serviceFriendsResponse = (Infrastructure.FriendsIdentityResponse) serviceResponse;
                    foreach (var identityParam in serviceFriendsResponse.Friends.Params)
                    {
                        response.Friends.Params.Add( identityParam.Clone() );
                    }
                }

                response.IsHeld = false;
            });
            
            return response;
        }

        [ServiceEventHandler(Event.OperationCode.Social.FriendsStatus)]
        private void OnFriendsStatusEvent(IServiceEvent serviceEvent)
        {
            var statusEvent = (Infrastructure.FriendsStatusEvent) serviceEvent;
            
            var destinationUserId = statusEvent.DestinationUserId.Value;
            var friend = statusEvent.Friend.Value;
            
            _context.ForEachPeer((peer) =>
            {
                  var actor = peer.GetActor();
                  if (actor != null && actor.Profile.UserId == destinationUserId)
                  {
                      Log.Info("friends :: friend status received for user id = " + destinationUserId + "; friend = " + friend);

                      var infoEvent = NetworkPool.GetOperation<FriendsStatusEvent>();
                      infoEvent.Friend.Value = friend;
                      peer.Send( infoEvent );
                  }
            });
        }
        
        [ServiceEventHandler(Event.OperationCode.Social.UserStatus)]
        private void OnUserStatusEvent(IServiceEvent serviceEvent)
        {
            var statusEvent = (Infrastructure.UserStatusEvent) serviceEvent;
            
            var destinationUserId = statusEvent.DestinationUserId.Value;
            var status = statusEvent.Status.Value;
            
            _context.ForEachPeer((peer) =>
            {
                var actor = peer.GetActor();
                if (actor != null && actor.Profile != null && actor.Profile.UserId == destinationUserId)
                {
                    Log.Info("friends :: user status received for user id = " + destinationUserId + "; about user id = " + status.UserId + "; state = " + status.State);
                    var infoEvent = NetworkPool.GetOperation<UserStatusEvent>();
                    infoEvent.Status.Value = status;
                    peer.Send( infoEvent );
                }
            });
        }
    }
}