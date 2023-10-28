using System;

namespace Bro.Client
{
    public class DirectEventObserver<T> : IEventObserver where T : DirectEvent
    {
        private WeakDelegate<T> _eventHandler;
        private WeakDelegate<DirectEventObserver<T>> _unsubscribeHandler;
        private bool IsSubscribed => _eventHandler != null;

        public DirectEventObserver(Action<DirectEventObserver<T>> unsubscribeHandler, Action<T> eventHandler)
        {
            _unsubscribeHandler = new WeakDelegate<DirectEventObserver<T>>(unsubscribeHandler);
            _eventHandler = new WeakDelegate<T>(eventHandler);
        }

        public void Unsubscribe()
        {
            if (_unsubscribeHandler != null)
            {
                _unsubscribeHandler.Invoke(this);
                _unsubscribeHandler = null;
            }
            _eventHandler = null;
        }

        void IDisposable.Dispose()
        {
            Unsubscribe();
        }

        public int EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        bool IEventObserver.OnEvent(IEvent e)
        {
            bool isObjectAlive = false;
            if (IsSubscribed)
            {
                isObjectAlive = _eventHandler.Invoke((T) e);
            }
            return isObjectAlive;
        }
    }
}