using System;

namespace Bro.Sketch.Client
{
    public class InventoryObserver<T> : IDisposable
    {
        private readonly InventoryModule _inventory;
        private readonly short _inventoryItemCode;
        private readonly InventoryItem<T>.ChangeValue _onChange;
        public InventoryObserver(InventoryModule inventory, short inventoryItemCode, InventoryItem<T>.ChangeValue onChange)
        {
            _inventory = inventory;
            _inventoryItemCode = inventoryItemCode;
            _onChange = onChange;
            _inventory.Register<T>(_inventoryItemCode, _onChange);
        }

        public void Register()
        {
            _inventory.Register<T>(_inventoryItemCode, _onChange);
        }
        
        private void Unregister()
        {
            _inventory.Unregister<T>(_inventoryItemCode, _onChange);
        }
        
        public void Dispose()
        {
            Unregister();
        }

        ~InventoryObserver()
        {
            Unregister();
        }
    }
}