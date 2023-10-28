using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class InventorySyncRequest : Bro.Network.NetworkRequest<InventorySyncRequest>
    {
        public readonly DictionaryParam<ShortParam, UniversalParam> Items = new DictionaryParam<ShortParam, UniversalParam>(byte.MaxValue, isOptional: true);

        public InventorySyncRequest() : base(Request.OperationCode.InventorySync)
        {
            AddParam(Items);
        }
    }
}