
using System;

namespace Bro.Client
{
    public class DirectEvent: IEvent
    {   
        private readonly IEventObserverAggregator _eventObserverAggregator = new EventObserverAggregator();

        public DirectEventObserver<T> Subscribe<T>(Action<T> handler) where T : DirectEvent
        {
            var result = new DirectEventObserver<T>(Unsubscribe, handler);
            _eventObserverAggregator.Add(result);
            return result;
        }

        private void Unsubscribe<T>(DirectEventObserver<T> e)  where T : DirectEvent
        {
            _eventObserverAggregator.Remove(e);
        }
        
        public void Launch()
        {
            _eventObserverAggregator.Invoke(this);
        }
    }
}