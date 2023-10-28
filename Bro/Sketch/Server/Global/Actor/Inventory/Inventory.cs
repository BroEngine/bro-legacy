using Bro.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Bro.Sketch.Server
{
    public class Inventory
    {
        [Flags]
        public enum PropertyType
        {
            Default = 1,
            ReadOnlyForClient = 2,
            AlwaysHasChangesAfterSet = 4,
        }

        private readonly Actor _actor;
        private readonly Dictionary<short, IInventoryItem> _items = new Dictionary<short, IInventoryItem>();
        public IReadOnlyDictionary<short, IInventoryItem> Items => _items;

        public object Data;
        public Actor Actor => _actor;

        public Inventory(Actor actor)
        {
            _actor = actor;
        }

        public void RegisterItem(IInventoryItem item)
        {
            if (!_items.ContainsKey(item.ItemId))
            {
                _items.Add(item.ItemId, item);
            }
            else
            {
                Bro.Log.Error($"inventory :: register item key = {item.ItemId.ToString()} is already added");
            }
        }

        public void SyncData(bool fullSync = false)
        {
            var inventoryModule = _actor.Peer.Context?.GetModule<InventoryModule>();
            if (inventoryModule != null)
            {
                inventoryModule.SyncData(_actor, fullSync);
            }
            else
            {
                Bro.Log.Warning($"inventory :: cannot send inventory changes, no inventory module in context {( _actor.Peer.Context != null ? _actor.Peer.Context.GetType().ToString() : "null")  }");
            }
        }

        public void InstantSyncData(bool fullSync = false)
        {
            var inventoryModule = _actor.Peer.Context?.GetModule<InventoryModule>();
            if (inventoryModule != null)
            {
                inventoryModule.InstantSyncData(_actor, fullSync);
            }
            else
            {
                Bro.Log.Warning($"inventory :: cannot send inventory changes, no inventory module in context {( _actor.Peer.Context != null ? _actor.Peer.Context.GetType().ToString() : "null")  }");
            }
        }

        public void SetItemValue(short itemKey, object itemValue)
        {
            if (_items.TryGetValue(itemKey, out var item))
            {
                if (!item.IsReadOnlyForClient)
                {
                    item.Value = itemValue;
                }
            }
            else
            {
                Bro.Log.Error($"inventory :: cannot find inventory item with key = {item.ItemId.ToString()} is already added");
            }
        }
    }
}