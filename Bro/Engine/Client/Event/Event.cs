namespace Bro.Client
{
    public class Event: IEvent
    {
        public readonly int EventId;

        public static int GetEventId<T>() where T : IEvent
        {
            return typeof(T).GetHashCode();
        }

        protected Event()
        {
            EventId = GetType().GetHashCode();
        }

        public virtual void Launch()
        {
            EventDispatcher.Instance.Dispatch(this);
        }
    }
}