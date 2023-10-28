using System;
using System.Collections.Generic;
using Bro.Network;
using Bro.Network.TransmitProtocol;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class InventoryModule : IServerContextModule
    {
        private class SyncQueue
        {
            private readonly List<IClientPeer> _regularSyncPeers = new List<IClientPeer>();
            private readonly List<IClientPeer> _fullSyncPeers = new List<IClientPeer>();

            public bool IsEmpty { get { return _fullSyncPeers.Count == 0 && _regularSyncPeers.Count == 0; } }

            public void AddSync(IClientPeer peer, bool isFullSync)
            {
                if (isFullSync)
                {
                    if (_fullSyncPeers.Contains(peer))
                    {
                        return;
                    }
                    _fullSyncPeers.Add(peer);
                }
                else
                {
                    if (_regularSyncPeers.Contains(peer))
                    {
                        return;
                    }
                    _regularSyncPeers.Add(peer);
                }
            }

            public bool IsFullSync(IClientPeer peer)
            {
                return _fullSyncPeers.Contains(peer);
            }

            public bool IsRegularSync(IClientPeer peer)
            {
                return _regularSyncPeers.Contains(peer);
            }

            public void Reset()
            {
                _fullSyncPeers.Clear();
                _regularSyncPeers.Clear();
            }

            public void RemoveByPeer(IClientPeer peer)
            {
                var curIndex = -1;
                
                if ((curIndex = _regularSyncPeers.IndexOf(peer)) != -1)
                {
                    _regularSyncPeers.RemoveAt(curIndex);
                    return;
                }
                
                if ((curIndex = _fullSyncPeers.IndexOf(peer)) != -1)
                {
                    _fullSyncPeers.RemoveAt(curIndex);
                }
            }
        }

        private IServerContext _context;
        private readonly SyncQueue _syncQueue = new SyncQueue();
        private event Action<IClientPeer, short, object> _onInventoryItemChanged;

        void IServerContextModule.Initialize(IServerContext context)
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
                        AttributeType = typeof(InventoryItemChangedHandlerAttribute),
                        AttachHandler = d => _onInventoryItemChanged += (Action<IClientPeer, short, object>) d,
                        HandlerType = typeof(Action<IClientPeer, short, object>)
                    }
                };
            }
        }

        [RequestHandler(Request.OperationCode.InventorySync)]
        private INetworkResponse OnReceiveInventorySyncRequest(INetworkRequest request, IClientPeer peer)
        {
            var inventorySyncRequest = (InventorySyncRequest) request;
            var inventory = peer.GetActor().Inventory;

            foreach (var itemParam in inventorySyncRequest.Items.Params)
            {
                var itemKey = itemParam.Key.Value;
                var itemValue = itemParam.Value.Value;
                inventory.SetItemValue(itemKey, itemValue);
                if (inventory.Items[itemKey].HasChanges)
                {
                    _onInventoryItemChanged?.Invoke(peer, itemKey, itemValue);
                }
            }

            return NetworkOperationFactory.CreateResponse(request);
        }

        [UpdateHandler(updatePeriod: GameConfig.Inventory.CheckSyncPeriodTimestamp)]
        private void UpdateCheckSync()
        {
            _context.ForEachPeer((peer) =>
            {
                var actor = peer.GetActor();
                if (actor != null)
                {
                    SendSyncEvent(actor, fullSync: false);
                }
            });
        }
        
        [PeerLeftHandler]
        private void OnPeerLeft(IClientPeer peer)
        {
            _syncQueue.RemoveByPeer(peer);
        }

        [UpdateHandler(updatePeriod: GameConfig.Inventory.SendSyncPeriodTimestamp)]
        private void UpdateSendSync()
        {
            if (!_syncQueue.IsEmpty)
            {
                _context.ForEachPeer((p) =>
                {
                    var curActor = p.GetActor();
                    if (curActor != null)
                    {
                        if (_syncQueue.IsFullSync(p))
                        {
                            SendSyncEvent(curActor, fullSync: true);
                        }
                        else if (_syncQueue.IsRegularSync(p))
                        {
                            SendSyncEvent(curActor, fullSync: false);
                        }

                        _syncQueue.RemoveByPeer(p);
                    }
                });
            }
        }

        private void SendSyncEvent(Actor actor, bool fullSync)
        {
            var inventory = actor.Inventory;

            if (!fullSync)
            {
                var hasAnyChanges = false;
                foreach (var item in inventory.Items)
                {
                    if (item.Value.HasChanges)
                    {
                        hasAnyChanges = true;
                        break;
                    }
                }

                if (!hasAnyChanges)
                {
                    return;
                }
            }

            short lastItemId = -1;
            try
            {
                var syncEvent = NetworkPool.GetOperation<InventorySyncEvent>();
                foreach (var item in inventory.Items)
                {
                    if (fullSync || item.Value.HasChanges)
                    {
                        lastItemId = item.Key;
                        syncEvent.Items.Add
                        (
                            new ShortParam() {Value = lastItemId},
                            new UniversalParam() {Value = item.Value.Value}
                        );
                        item.Value.OnSync();
                    }
                }
                actor.Peer.Send(syncEvent);
            }
            catch (Exception ex)
            {
                Bro.Log.Error($"inventory module :: exception during send sync event last item id ={lastItemId}; {ex}");
            }            
        }

        public void SyncData(Actor actor, bool fullSync = false)
        {
            _syncQueue.AddSync(actor.Peer, fullSync);
        }

        public void InstantSyncData(Actor actor, bool fullSync = false)
        {
            SendSyncEvent(actor, fullSync);
        }
    }
}