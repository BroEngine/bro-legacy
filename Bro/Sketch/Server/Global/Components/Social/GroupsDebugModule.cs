using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class GroupsDebugModule : IServerContextModule
    {
        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;
        
        private IServerContext _context;
        private GroupsModule _groupsModule;

        private readonly List<Actor> _enabledActors = new List<Actor>();
        
        public void Initialize(IServerContext context)
        {
            _context = context;
            _groupsModule = _context.GetModule<GroupsModule>();
        }
        
        [PeerLeftHandler]
        private void OnPeerLeft(IClientPeer peer)
        {
            var actor = peer.GetActor();
            _enabledActors.Remove(actor);
        }
        
        [RequestHandler(Request.OperationCode.Social.GroupDebug)]
        private INetworkResponse OnGroupDebugRequest(INetworkRequest request, Bro.Server.Network.IClientPeer peer)
        {
            var actor = peer.GetActor();
            var userId = actor.Profile.UserId;

            var debugRequest = (GroupsDebugRequest) request;
            var argument = debugRequest.Argument.Value;
            var data = debugRequest.Data.Value;
            var command = (GroupDebug)debugRequest.Command.Value;
            
            Log.Info("groups :: debug request received from user = " + userId + "; command = " + command  + "; argument = " + argument);

            switch (command)
            {
                case GroupDebug.Enable:
                    if (!_enabledActors.Contains(actor))
                    {
                        _enabledActors.Add(actor);
                    }
                    _groupsModule.RequestParty(actor);
                    break;
                case GroupDebug.Disable:
                    _enabledActors.Remove(actor);
                    break;
                case GroupDebug.Create:
                    _groupsModule.Create(actor, data);
                    break;
                case GroupDebug.Leave:
                    _groupsModule.Leave(actor);
                    break;
                case GroupDebug.Add:
                    _groupsModule.Add(actor, argument);
                    break;
                case GroupDebug.Remove:
                    _groupsModule.Remove(actor, argument);
                    break;     
                case GroupDebug.Update:
                    _groupsModule.Update(actor, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return null;
        }

        [GroupChangedHandler]
        private void OnUsersGroupChange(Actor actor, Group group)
        {
            if (_enabledActors.Contains(actor))
            {
                var peer = actor.Peer;
                var infoEvent = NetworkPool.GetOperation<GroupsDebugEvent>();
                if (group != null)
                {
                    infoEvent.Group.Value = group;
                }

                peer.Send( infoEvent );
            }
        }
    }
}