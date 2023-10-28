namespace Bro.Sketch.Server
{
    public abstract class InventoryItem<T> : IInventoryItem
    {
        protected abstract T _value { get; set; }
        private readonly Inventory _inventory;

        private readonly short _itemId;
        private readonly bool _isReadOnlyForClient;
        private readonly bool _isAlwaysHasChangesAfterSet;

        public bool HasChanges { get; private set; }

        public T Value
        {
            get => _value;
            set
            {
                if ((_value == null && value != null) || (_value != null && !_value.Equals(value)))
                {
                    _value = value;
                    HasChanges = true;
                }

                if (_isAlwaysHasChangesAfterSet)
                {
                    HasChanges = true;
                }

                if (HasChanges)
                {
                    _inventory.SyncData();
                }
            }
        }

        protected InventoryItem(Inventory inventory, short itemId, Inventory.PropertyType propertyType)
        {
            HasChanges = true;
            _inventory = inventory;
            _itemId = itemId;
            _isReadOnlyForClient = propertyType.HasFlag(Inventory.PropertyType.ReadOnlyForClient);
            _isAlwaysHasChangesAfterSet = propertyType.HasFlag(Inventory.PropertyType.AlwaysHasChangesAfterSet);
            _inventory.RegisterItem(this);
        }

        public void MakeChanged()
        {
            HasChanges = true;
        }

        short IInventoryItem.ItemId => _itemId;

        bool IInventoryItem.IsReadOnlyForClient => _isReadOnlyForClient;

        object IInventoryItem.Value
        {
            get => Value;
            set => Value = (T) value;
        }

        void IInventoryItem.OnSync()
        {
            HasChanges = false;
        }
    }
}