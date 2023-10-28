using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class AuthorizationModule : IServerContextModule
    {
        private event Action<IClientPeer> _onAuthorizationComplete;

        private IServerContext _context;
        private IProfileProvider _profileProvider;
        private IInventoryDataProvider _inventoryDataProvider;
        private IConfigHolderProvider _configHolderProvider;

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
            _profileProvider = context.GetModule(c => c is IProfileProvider) as IProfileProvider;
            Bro.Log.Assert(_profileProvider != null, "authorization module :: profile provider is null");

            _inventoryDataProvider = context.GetModule(c => c is IInventoryDataProvider) as IInventoryDataProvider;
            Bro.Log.Assert(_inventoryDataProvider != null, "authorization module :: inventory data provider is null");
            
            _configHolderProvider = context.GetModule(c => c is IConfigHolderProvider) as IConfigHolderProvider;
            Bro.Log.Assert(_configHolderProvider != null, "authorization module :: config holder provider is null");
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers
        {
            get
            {
                return new List<CustomHandlerDispatcher.HandlerInfo>()
                {
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(AuthorizationCompleteHandlerAttribute),
                        AttachHandler = d => _onAuthorizationComplete += (Action<Bro.Server.Network.IClientPeer>) d,
                        HandlerType = typeof(Action<Bro.Server.Network.IClientPeer>)
                    }
                };
            }
        }
        
        [RequestHandler(Request.OperationCode.Authorization)]
        private INetworkResponse OnAuthorizationRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            Bro.Log.Info("authorization module :: authorization request from peer id = " + peer.PeerId);

            var authorizationRequest = (AuthorizationRequest) request;
            var deviceId = authorizationRequest.DeviceId.Value;
            var platform = (PlatformType) authorizationRequest.Platform.Value;
            var sessionId = authorizationRequest.SessionId.Value;

            var resetProfile = authorizationRequest.ResetProfile.Value;
            
            var googleId = authorizationRequest.GoogleId.IsInitialized ? authorizationRequest.GoogleId.Value : string.Empty;
            var appleId = authorizationRequest.AppleId.IsInitialized ? authorizationRequest.AppleId.Value : string.Empty;
            var facebookId = authorizationRequest.FacebookId.IsInitialized ? authorizationRequest.FacebookId.Value : string.Empty;
            var userIdentity = new UserIdentity(platform, deviceId, googleId, appleId, facebookId);

            var isQueueBusy = _profileProvider.IsQueueBusy();

            var response = (AuthorizationResponse) NetworkOperationFactory.CreateResponse(request);
            response.ErrorCode.Value = 0;
            response.UserId.Value = 0;
            response.IsHeld = true;
            response.IsAuthorized.Value = false;
            response.Queued.Value = false;

            var actor = peer.GetActor();

            if (isQueueBusy)
            {
                response.Queued.Value = true;
                response.IsHeld = false;
            }
            else
            {
                var authStarted = _profileProvider.Load(userIdentity, actor.Peer, resetProfile, (profile, errorCode) =>
                {
                    response.ErrorCode.Value = errorCode;
                    
                    if (profile == null)
                    {
                        response.IsAuthorized.Value = false;
                        response.IsHeld = false;
                    }
                    else
                    {
                        actor.Profile = profile;
                        actor.ConfigHolder = _configHolderProvider.CreateConfigHolder(profile);
                        actor.Inventory.Data = _inventoryDataProvider.CreateInventoryData(actor);
                        
                        response.IsAuthorized.Value = true;
                        response.UserId.Value = actor.Profile.UserId;
                        response.SetConfigDetails(actor.ConfigHolder.DetailsCatalog);

                        _onAuthorizationComplete?.Invoke(peer);
                        actor.Inventory.InstantSyncData(true);

                        response.IsHeld = false;
                    }
                });

                if (!authStarted)
                {
                    response.Queued.Value = true;
                    response.IsHeld = false;
                }
            }

            return response;
        }
    }
}