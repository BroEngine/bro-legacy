namespace Bro.Sketch.Server
{
    public interface IInventoryItem
    {
        short ItemId { get; }
        bool IsReadOnlyForClient { get; }
        bool HasChanges { get; }
        
        object Value { get; set; }
        void OnSync();
        
        
    }
}