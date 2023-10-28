namespace Bro.Sketch.Server
{
    public class SimpleInventoryItem<T> : InventoryItem<T>
    {
        protected override T _value { get; set; }
        
        public SimpleInventoryItem(Inventory inventory, short itemId, Inventory.PropertyType propertyType) : base(inventory, itemId, propertyType)
        {
        }
    }
}