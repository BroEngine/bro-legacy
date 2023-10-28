using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class InventorySyncEvent : Bro.Network.NetworkEvent<InventorySyncEvent>
    {
        public readonly DictionaryParam<ShortParam, UniversalParam> Items = new DictionaryParam<ShortParam, UniversalParam>(byte.MaxValue, isOptional: true);
        
        public InventorySyncEvent() : base(Event.OperationCode.InventorySync)
        {
            AddParam(Items);
        
        }
    }
}