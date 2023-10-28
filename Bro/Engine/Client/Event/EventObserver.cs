using System;

namespace Bro.Client
{
    /// <summary>
    /// Always save object reference otherwise garbage collector will destroy observer
    /// </summary>
    /// <typeparam name="T">Event type you are going to subscribe</typeparam>
    public class EventObserver<T>:  IEventObserver where T : IEvent
    {
        private WeakDelegate<T> _eventHandler;
        private readonly int _eventId;
        int IEventObserver.EventId => _eventId;
        private bool IsSubscribed => _eventHandler != null;
        
        public EventObserver()
        {
            _eventId = Event.GetEventId<T>();
        }

        public EventObserver(Action<T> handler) : this()
        {
            Subscribe(handler);
        }

        ~EventObserver()
        {
            Unsubscribe();
        }

        public void Subscribe(Action<T> handler)
        {
            Unsubscribe();
            _eventHandler = new WeakDelegate<T>(handler);
            EventDispatcher.Instance.Register(this);
        }

        public void Unsubscribe()
        {
            if (_eventHandler != null)
            {
                EventDispatcher.Instance.Unregister(this);
                _eventHandler = null;
            }
        }

        bool IEventObserver.OnEvent(IEvent e)
        {
            bool isObjectAlive = false;
            if (IsSubscribed)
            {
                isObjectAlive = _eventHandler.Invoke((T) e);
            }
            return isObjectAlive;
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}