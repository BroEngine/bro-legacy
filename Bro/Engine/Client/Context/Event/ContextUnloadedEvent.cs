
namespace Bro.Client
{
    public class ContextUnloadedEvent : Event
    {
        public readonly IClientContext Context;
        
        public ContextUnloadedEvent(IClientContext context)
        {
            Context = context;
        }
    }
}