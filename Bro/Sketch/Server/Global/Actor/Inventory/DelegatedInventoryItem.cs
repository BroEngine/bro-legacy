using System;

namespace Bro.Sketch.Server
{
    public class DelegatedInventoryItem<T> : InventoryItem<T>
    {
        public delegate T GetDelegate();

        public delegate void SetDelegate(T value);

        private readonly GetDelegate _getValue;
        private readonly SetDelegate _setValue;

        public DelegatedInventoryItem(Inventory inventory, short itemId, GetDelegate get, SetDelegate set,
            Inventory.PropertyType propertyType = Inventory.PropertyType.Default) : base(inventory, itemId, propertyType)
        {
            _getValue = get;
            _setValue = set;
        }

        protected override T _value
        {
            get { return _getValue(); }
            set { _setValue(value); }
        }
    }
}