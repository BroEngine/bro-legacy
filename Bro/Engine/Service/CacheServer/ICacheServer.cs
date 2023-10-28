using System;
using System.Collections.Generic;

namespace Bro.Service
{
    public interface ICacheServer
    {
        bool IsStarted { get; }
        bool IsConnected { get; }

        void Start(CacheServerConfig cacheServerConfig);
        
        void Set(string key, string value, int ttl);
        void Get(string key, Action<string> callback);
        void Get(List<string> keys, Action<List<string>> callback);
        void Transaction(string key, string value, int ttl, int attempts, Action<bool> callback);
        void Transaction(string key, int ttl, int attempts, CacheServerDelegate.ValueJsonCheck checker, CacheServerDelegate.ValueJsonCreate creator, Action<bool> callback);
    }
}