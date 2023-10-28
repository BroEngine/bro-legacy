namespace Bro.Sketch.Network
{
    public class InventorySyncResponse : Bro.Network.NetworkResponse<InventorySyncResponse>
    {
        public InventorySyncResponse() : base(Request.OperationCode.InventorySync)
        {
        }
    }
}