using System;
using System.Collections.Generic;
using Bro.Network.Service;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Service;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class GroupsModule : IServerContextModule
    {
        private IServerContext _context;
        private event Action<Actor, Group> _onUsersGroupChanged;
        
        private readonly Dictionary<Actor, Group> _groups = new Dictionary<Actor, Group>();
            
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
                        AttributeType = typeof(GroupChangedHandlerAttribute),
                        AttachHandler = d => _onUsersGroupChanged += (Action<Actor, Group>) d,
                        HandlerType = typeof(Action<Actor, Group>)
                    }
                };
            }
        }

        public Group GetGroup(Actor actor) // null if no resolved
        {
            if (_groups.ContainsKey(actor))
            {
                var group = _groups[actor];
                var userId = actor.Profile.UserId;
                
                if (!group.Users.Contains(userId))
                {
                    group = new Group(); // special
                }
                
                return group;
            }

            return null;
        }

        [PeerJoinedHandler]
        private void OnPeerJoin(IClientPeer peer, object data)
        {
            var actor = peer.GetActor();
            RequestParty(actor);
        }

        [PeerLeftHandler]
        private void OnPeerLeft(IClientPeer peer)
        {
            var actor = peer.GetActor();
            _groups.Remove(actor);
        }
        
        public void RequestParty(Actor actor)
        {
            var userId = actor.Profile.UserId;
            var serviceRequest = new Infrastructure.GroupGetRequest();
            serviceRequest.UserId.Value = userId;
            
            _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
            {
                if (serviceResponse != null)
                {
                    var response = (Infrastructure.GroupGetResponse) serviceResponse;
                    var group = response.Group.Value;
                    
                    var actualActor = _context.GetActor(actor.Profile.UserId);
                    if (actualActor != null)
                    {
                        _groups[actor] = group;
                        _onUsersGroupChanged?.Invoke(actor, group);
                    }

                    Log.Info("groups :: get party service request handled for user id = " + userId + "; group = " + group);
                }
                else
                {
                    Log.Info("groups :: get party service request failed for user id = " + userId);
                }
            });
        }

        [ServiceEventHandler(Event.OperationCode.Social.GroupStatus)]
        private void OnGroupStatusEvent(IServiceEvent serviceEvent)
        {
            var statusEvent = (Infrastructure.GroupStatusEvent) serviceEvent;
            var userId = statusEvent.DestinationUserId.Value;
            var group = statusEvent.Group.Value;
            var actor = _context.GetActor(userId);

            if (actor != null)
            {
                Log.Info("groups :: group info status received for user id = " + userId + "; group = " + group);

                if (!group.Users.Contains(userId))
                {
                    group = new Group(); // special
                }

                _groups[actor] = group;
                _onUsersGroupChanged?.Invoke(actor, group);
            }
        }

        public void Leave(Actor actor)
        {
            var userId = actor.Profile.UserId;
            var serviceRequest = new Infrastructure.GroupLeaveRequest();
            serviceRequest.UserId.Value = userId;
            _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
            {
                Log.Info("groups :: leave group for user id = " + userId + " result = " + (serviceResponse != null));
            });
        }
        
        public void Create(Actor actor, string data = "", Action<Group> callback = null)
        {
            var userId = actor.Profile.UserId;
            
            Bro.Log.Info("groups :: created called for " + userId);
            
            if (_groups.ContainsKey(actor))
            {
                var group = _groups[actor];
                if (group == null || group.GroupId == 0)
                {
                    group = new Group();
                    group.Users.Add( userId );
                    group.Data = data;
                    
                    var serviceRequest = new Infrastructure.GroupSetRequest(group);
                    
                    _context.SendServiceRequest(serviceRequest, (serviceResponse) =>
                    {
                        if (serviceResponse != null)
                        {
                            var response = (Infrastructure.GroupSetResponse)serviceResponse;
                            group = response.Group.Value;

                            if (group != null)
                            {
                                Log.Info("groups :: create request completed for user id = " + userId + ", group id = " + group.GroupId);
                                 // _groups[actor] = group;
                                callback?.Invoke(group);
                            }
                            else
                            {
                                Log.Info("groups :: create request failed on null response for user id = " + userId);
                                callback?.Invoke(null);
                            }
                        }
                        else
                        {
                            Log.Info("groups :: create request failed for user id = " + userId);
                            callback?.Invoke(null);
                        }
                    });
                }
                else
                {
                    Bro.Log.Error("groups :: not possible to create a group because actor already in group id = " + group.GroupId);   
                }
            }
            else
            {
                Bro.Log.Error("groups :: not possible to create a group because the actors status does not exist");
            }
        }

        public void Remove(Actor actor, int userIdToRemove)
        {
            var userId = actor.Profile.UserId;
            
            Bro.Log.Info("groups :: remove called from " + userId + " to user id " + userIdToRemove);

            if (_groups.ContainsKey(actor))
            {
                var group = _groups[actor];
                if (group != null)
                {
                    if (group.Users.Contains(userIdToRemove))
                    {
                        // request
                        group.Users.Remove(userIdToRemove);
                        
                        var serviceRequest = new Infrastructure.GroupSetRequest(group);
                        _context.SendServiceRequest(serviceRequest, null);
                        // request
                    }
                    else
                    {
                        Bro.Log.Error("groups :: not possible to remove user id = " + userIdToRemove + " because he is not in the group");
                    }
                }
                else
                {
                    Bro.Log.Error("groups :: not possible to remove from group because actor is not in group");   
                }
            }
            else
            {
                Bro.Log.Error("groups :: not possible to remove from group because the actors status does not exist");
            }
        }

        public void Add(Actor actor, int userIdToAdd)
        {
            var userId = actor.Profile.UserId;
            
            Bro.Log.Info("groups :: add called from " + userId + " to user id " + userIdToAdd);

            if (_groups.ContainsKey(actor))
            {
                var group = _groups[actor];
                if (group != null)
                {
                    if (!group.Users.Contains(userIdToAdd))
                    {
                        // request
                        group.Users.Add(userIdToAdd);
                        var serviceRequest = new Infrastructure.GroupSetRequest(group);
                        _context.SendServiceRequest(serviceRequest, null);
                        // request
                    }
                    else
                    {
                        Bro.Log.Error("groups :: not possible to add user id = " + userIdToAdd + " because he is already in group");
                    }
                }
                else
                {
                    Bro.Log.Error("groups :: not possible to add to the group because actor is not in group");     
                }
            }
            else
            {
                Bro.Log.Error("groups :: not possible to add to the group because the actors status does not exist");
            }
        }

        public void Update(Actor actor, Group group)
        {
            var userId = actor.Profile.UserId;
            
            Bro.Log.Info("groups :: update called from user id = " + userId);

            if (_groups.ContainsKey(actor))
            {
                if (group != null)
                {
                    // request
                    var serviceRequest = new Infrastructure.GroupSetRequest(group);
                    _context.SendServiceRequest(serviceRequest, null);
                    // request
                }
                else
                {
                    Bro.Log.Error("groups :: not possible to update the group because actor is not in group");     
                }
            }
            else
            {
                Bro.Log.Error("groups :: not possible to update the group because the actors status does not exist");
            }
        }
        

        public void Update(Actor actor, string data)
        {
            var userId = actor.Profile.UserId;
            
            Bro.Log.Info("groups :: update called from user id = " + userId);

            if (_groups.ContainsKey(actor))
            {
                var group = _groups[actor];
                if (group != null)
                {
                    group.Data = data;
                    // request
                    var serviceRequest = new Infrastructure.GroupSetRequest(group);
                    _context.SendServiceRequest(serviceRequest, null);
                    // request
                }
                else
                {
                    Bro.Log.Error("groups :: not possible to update the group because actor is not in group");     
                }
            }
            else
            {
                Bro.Log.Error("groups :: not possible to update the group because the actors status does not exist");
            }
        }
    }
}