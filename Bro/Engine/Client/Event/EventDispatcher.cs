using System;
using System.Collections.Generic;
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using UnityEngine;

#endif

namespace Bro.Client
{
    public sealed class EventDispatcher
    {
        private static EventDispatcher dispatcher = new EventDispatcher();

        public static EventDispatcher Instance => dispatcher;

        private readonly Dictionary<int, IEventObserverAggregator> _eventObserverAggregators = new Dictionary<int, IEventObserverAggregator>();

        public void Dispatch<T>(T incomingEvent) where T : Event
        {
            IEventObserverAggregator wrapper;
            _eventObserverAggregators.FastTryGetValue(incomingEvent.EventId, out wrapper);
            var foundHandlerAndHandlerHasDelegates = (wrapper != null);
            if (foundHandlerAndHandlerHasDelegates)
            {
                wrapper.Invoke(incomingEvent);
            }
        }

        public void Register(IEventObserver observer)
        {
            var eventId = observer.EventId;
            IEventObserverAggregator wrapper;
            if (!_eventObserverAggregators.TryGetValue(eventId, out wrapper))
            {
                wrapper = new EventObserverAggregator();
                _eventObserverAggregators[eventId] = wrapper;
            }

            wrapper.Add(observer);
        }

        public void Unregister(IEventObserver observer)
        {
            int eventHashId = observer.EventId;
            IEventObserverAggregator wrapper;
            if (_eventObserverAggregators.TryGetValue(eventHashId, out wrapper))
            {
                wrapper.Remove(observer);
            }
        }
    }
}