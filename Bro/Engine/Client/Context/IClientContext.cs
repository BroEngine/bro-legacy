using System;
using Bro.Network;

namespace Bro.Client
{
    public interface IClientContext : IWebContext, ITaskContext
    {
        bool IsAlive { get; }
        ClientApplication Application { get; }
        void Load(ClientApplication application, Action onFinish = null);
        void Unload(Action onFinish = null);
        Timing.IScheduler Scheduler { get; }
        T GetModule<T>() where T : class, IClientContextModule;
        
        void AddDisposable(IDisposable disposable);
    }
}