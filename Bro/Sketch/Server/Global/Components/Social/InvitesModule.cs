using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Network.Service;
using Bro.Server.Context;
using Bro.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class InvitesModule<T> : IServerContextModule where T : new()
    {
        private IServerContext _context;
        
        private event Action<Actor, InviteAction, int, T> _onEvent;
        private event Action<Actor, int, T> _onInvite;
        private event Action<Actor, int, T> _onCancel;
        private event Action<Actor, int, T> _onAccept;
        private event Action<Actor, int, T> _onDeny;
        
        public void Initialize(IServerContext context)
        {
            _context = context;
        }
        
        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers
        {
            get
            {
                return new List<CustomHandlerDispatcher.HandlerInfo>()
                {
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(InviteEventHandlerAttribute),
                        AttachHandler = d => _onEvent += (Action<Actor, InviteAction, int, T>) d,
                        HandlerType = typeof(Action<Actor, InviteAction, int, T>)
                    },
                    
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(InviteHandlerAttribute),
                        AttachHandler = d => _onInvite += (Action<Actor, int, T>) d,
                        HandlerType = typeof(Action<Actor, int, T>)
                    },
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(InviteAcceptHandlerAttribute),
                        AttachHandler = d => _onAccept += (Action<Actor, int, T>) d,
                        HandlerType = typeof(Action<Actor, int, T>)
                    },
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(InviteDenyHandlerAttribute),
                        AttachHandler = d => _onDeny += (Action<Actor, int, T>) d,
                        HandlerType = typeof(Action<Actor, int, T>)
                    },
                    new CustomHandlerDispatcher.HandlerInfo()
                    {
                        AttributeType = typeof(InviteCancelHandlerAttribute),
                        AttachHandler = d => _onCancel += (Action<Actor, int, T>) d,
                        HandlerType = typeof(Action<Actor, int, T>)
                    }
                };
            }
        }
        
        [ServiceEventHandler(Event.OperationCode.Social.Invite)]
        private void OnInviteEvent(IServiceEvent serviceEvent)
        {
            var inviteEvent = (Infrastructure.InviteEvent) serviceEvent;
            var targetUserId = inviteEvent.TargetUserId.Value;
            var authorUserId = inviteEvent.AuthorUserId.Value;
            var action = (InviteAction)inviteEvent.Action.Value;
            var json = inviteEvent.Data.Value;
            
            var actor = _context.GetActor(targetUserId);

            if (actor != null)
            {
                Log.Info("invites :: service event received from = " +  authorUserId + " to = " + targetUserId + "; action = " + action);

                var data = Deserialize(json);
                
                switch (action)
                {
                    case InviteAction.Invite:
                        _onInvite?.Invoke(actor, authorUserId, data);
                        break;
                    case InviteAction.Cancel:
                        _onCancel?.Invoke(actor, authorUserId, data);
                        break;
                    case InviteAction.Accept:
                        _onAccept?.Invoke(actor, authorUserId, data);
                        break;
                    case InviteAction.Deny:
                        _onDeny?.Invoke(actor, authorUserId, data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _onEvent?.Invoke(actor, action, authorUserId, data);
            }
        }

        private static T Deserialize(string json)
        {
            if (json == null || string.IsNullOrEmpty(json))
            {
                return new T();
            }

            return JsonConvert.DeserializeObject<T>(json, JsonSettings.AutoSettings);
        }

        private static string Serialize(T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None, JsonSettings.AutoSettings);
            return json;
        }

        public void Invite(Actor actor, int targetUserId, T data)
        {
            SendRequest(actor, targetUserId, InviteAction.Invite, data);
        }
        
        public void Cancel(Actor actor, int targetUserId, T data)
        {
            SendRequest(actor, targetUserId, InviteAction.Cancel, data);
        }
        
        public void Accept(Actor actor, int targetUserId, T data)
        {
            SendRequest(actor, targetUserId, InviteAction.Accept, data);
        } 
        
        public void Deny(Actor actor, int targetUserId, T data)
        {
            SendRequest(actor, targetUserId, InviteAction.Deny, data);
        }

        private void SendRequest(Actor actor, int targetUserId, InviteAction action, T data)
        {
            var authorUserId = actor.Profile.UserId;
            var json = Serialize(data);
            
            var serviceRequest = new Infrastructure.InviteRequest();
            
            serviceRequest.TargetUserId.Value = targetUserId;
            serviceRequest.AuthorUserId.Value = authorUserId;
            serviceRequest.Action.Value = (byte) action;
            serviceRequest.Data.Value = json;
          
            Log.Info("invite :: user " + authorUserId + " request to " + targetUserId + " action = " + action + " data = " + json );

            _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
            {
                Log.Info("invite :: user " + authorUserId + " request to " + targetUserId + " action = " + action + " result = " + (serviceResponse != null) );
            });
        }
    }
}