using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class InventoryModule : IClientContextModule
    {
        private IClientContext _context;
        private NetworkEventObserver<InventorySyncEvent> _inventoryEventObserver;
        private readonly Dictionary<short, IInventoryItem> _items = new Dictionary<short, IInventoryItem>();

        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        void IClientContextModule.Initialize(IClientContext context)
        {
            _context = context;
        }

        IEnumerator IClientContextModule.Load()
        {
            _inventoryEventObserver = new NetworkEventObserver<InventorySyncEvent>(OnInventorySyncEvent, _context.GetNetworkEngine());
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            _inventoryEventObserver.Unsubscribe();
            _inventoryEventObserver = null;
            return null;
        }

        private void OnInventorySyncEvent(InventorySyncEvent e)
        {
            foreach (var itemParam in e.Items.Params)
            {
                var itemKey = itemParam.Key.Value;
                var itemValue = itemParam.Value.Value;
                if (_items.FastTryGetValue(itemKey, out var targetItem))
                {
                    targetItem.Value = itemValue;
                }
            }
        }

        public void Register<T>(short inventoryItemCode, InventoryItem<T>.ChangeValue onChange)
        {
            GetInventoryItem<T>(inventoryItemCode).OnChange += onChange;
        }

        public void Unregister<T>(short inventoryItemCode, InventoryItem<T>.ChangeValue onChange)
        {
            GetInventoryItem<T>(inventoryItemCode).OnChange -= onChange;
        }

        public void Sync()
        {
            var syncRequest = NetworkPool.GetOperation<InventorySyncRequest>();
            foreach (var item in _items)
            {
                syncRequest.Items.Add
                (
                    new ShortParam() {Value = item.Key},
                    new UniversalParam() {Value = item.Value.Value}
                );
            }

            new SendRequestTask(syncRequest, _context.GetNetworkEngine()).Launch(_context);
        }

        public InventoryItem<T> GetInventoryItem<T>(short inventoryItemCode)
        {
            InventoryItem<T> result;
            if (!_items.FastTryGetValue(inventoryItemCode, out var searchResult))
            {
                result = new InventoryItem<T>();
                _items.Add(inventoryItemCode, result);
            }
            else
            {
                result = (InventoryItem<T>) searchResult;
            }
            return result;
        }
    }
}