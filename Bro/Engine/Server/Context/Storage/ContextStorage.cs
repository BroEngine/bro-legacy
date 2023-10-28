using System;
using System.Collections.Generic;

namespace Bro.Server.Context
{
    public class ContextStorage
    {
        private readonly object _sync = new object();
        private readonly List<IServerContext> _activeContexts = new List<IServerContext>();
        private readonly OnlineCounter _onlineCounter = new OnlineCounter();

        public virtual IServerContext GetEntryContext()
        {
            throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
            
        }
        
        public virtual void OnContextInitialization(IServerContext context)
        {
          
        }

        public void OnContextStarted(IServerContext context)
        {
            lock (_sync)
            {
                if (!_activeContexts.Contains(context))
                {
                    _activeContexts.Add(context);
                }
            }
        }

        public void OnContextStopped(IServerContext context)
        {
            lock (_sync)
            {
                _activeContexts.Remove(context);
            }
        }

        public OnlineCounter.Model GetOnlineModel()
        {
            lock (_sync)
            {
                return _onlineCounter.Count(_activeContexts);
            }
        }

        public List<IServerContext> GetContexts()
        {
            var result = new List<IServerContext>();

            lock (_sync)
            {
                for (var i = 0; i < _activeContexts.Count; ++i)
                {
                    result.Add(_activeContexts[i]);
                }
            }

            return result;
        }
        
        public List<T> GetContexts<T>() where T : IServerContext
        {
            var result = new List<T>();

            lock (_sync)
            {
                for (var i = 0; i < _activeContexts.Count; ++i)
                {
                    if (_activeContexts[i] is T)
                    {
                        result.Add((T) _activeContexts[i]);
                    }
                }
            }

            return result;
        }
        
        public List<IServerContext> GetContexts(Type type)
        {
            var result = new List<IServerContext>();

            lock (_sync)
            {
                for (var i = 0; i < _activeContexts.Count; ++i)
                {
                    if (_activeContexts[i].GetType() == type)
                    {
                        result.Add(_activeContexts[i]);
                    }
                }
            }

            return result;
        }

        public IServerContext GetAnyContext<T>() where T : IServerContext
        {
            var results = GetContexts<T>();
            if (results.Count == 1)
            {
                return results[0];
            }

            if (results.Count > 1)
            {
                return results[Bro.Random.Instance.Range(0, results.Count)];
            }

            Bro.Log.Info("No any instance of context type = " + typeof(T));

            return null;
        } 
        
        public IServerContext GetAnyContext(Type type) 
        {
            var results = GetContexts(type);
            if (results.Count == 1)
            {
                return results[0];
            }

            if (results.Count > 1)
            {
                return results[Bro.Random.Instance.Range(0, results.Count)];
            }

            Bro.Log.Info("no any instance of context type = " + type);

            return null;
        }
        
        public int  TotalOnline
        {
            get
            {
                lock (_sync)
                {
                    var totalOnline = 0;
                    for (var i = 0; i < _activeContexts.Count; ++i)
                    {
                        totalOnline += _activeContexts[i].PeersAmount;
                    }
                    return totalOnline;
                }
            }
        }
        
    }
}