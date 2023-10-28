using System;
using System.Collections.Generic;

namespace Bro.Client
{
    public class EventObserverAggregator : IEventObserverAggregator
    {
        private readonly List<IEventObserver> _activeObservers = new List<IEventObserver>();
        private readonly List<IEventObserver> _observersForRecycle = new List<IEventObserver>();
        private readonly List<IEventObserver> _observersToAdd = new List<IEventObserver>();
        private int _callCounter;

        public void Invoke(IEvent e)
        {
            if (_callCounter == 0) // check not to recycle handlers during calling event in same event type
            {
                RecycleObservers();
                AddPendingObservers();
            }

            if (_activeObservers != null)
            {
                ++_callCounter;
                for (int i = 0, max = _activeObservers.Count; i < max; ++i)
                {
                    try
                    {
                        bool isAlive = _activeObservers[i].OnEvent(e);
                        if (!isAlive)
                        {
                            Remove(_activeObservers[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Bro.Log.Error($"exception with {_activeObservers[i].GetType()} {ex}");
                    }
                }

                --_callCounter;
            }
        }

        void IEventObserverAggregator.Add(IEventObserver observer)
        {
            if (_callCounter == 0) // add only if not calling event
            {
                _activeObservers.Add(observer);
            }
            else
            {
                _observersToAdd.Add(observer);
            }
        }

        public void Remove(IEventObserver observer)
        {
            if (_callCounter == 0) // check not to recycle handlers during calling event in same event type
            {
                _activeObservers.Remove(observer);
            }
            else if (!_observersForRecycle.Contains(observer))
            {
                _observersForRecycle.Add(observer);
            }
        }

        private void AddPendingObservers()
        {
            var pendingObserversCount = _observersToAdd.Count;
            if (pendingObserversCount > 0)
            {
                for (int i = 0; i < pendingObserversCount; ++i)
                {
                    _activeObservers.Add(_observersToAdd[i]);
                }

                _observersToAdd.Clear();
            }
        }
        
        private void RecycleObservers()
        {
            var observersForRecycleCount = _observersForRecycle.Count;
            if (observersForRecycleCount > 0)
            {
                for (int i = 0; i < observersForRecycleCount; ++i)
                {
                    _activeObservers.Remove(_observersForRecycle[i]);
                }

                _observersForRecycle.Clear();
            }
        }
    }
}