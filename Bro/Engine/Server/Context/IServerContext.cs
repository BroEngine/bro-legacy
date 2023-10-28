using System;
using Bro.Network;
using Bro.Network.Service;
using Bro.Server.Network;

namespace Bro.Server.Context
{
    public interface IServerContext : IPeerContainer, IWebContext, ITaskContext
    {
        void Initialize();
        void Start();
        void Stop();
        void Terminate();

        void HandleRequest(INetworkRequest request, IClientPeer peer, Action onComplete);

        /// <summary>
        /// Ask context if peer can join it, if yes, than join 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="onJoinCompleted"></param>
        /// <param name="data"></param>
        /// <returns>false if its not permitted to join now, true is OK</returns>
        bool Join(IClientPeer peer, System.Action onJoinCompleted = null, object data = null);
        void Leave(IClientPeer peer, System.Action onLeaveCompleted = null);
        void OnDisconnect(IClientPeer peer, Action onDisconnectCompleted);

        void SendServiceEvent( IServiceEvent serviceEvent );
        void SendServiceRequest( IServiceRequest serviceRequest, Action<IServiceResponse> callback );
        void SendServiceRequest( IServiceRequest serviceRequest );

        void AddModule(IServerContextModule module);
        T GetModule<T>() where T : class, IServerContextModule;
        IServerContextModule GetModule(Predicate<IServerContextModule> matchPredicate);

        ContextStorage ContextStorage { get; }
        ConfigStorageCollector ConfigStorageCollector { get; }
        IScheduler Scheduler { get; }

        IClientPeer GetPeer(Predicate<IClientPeer> predicate);
        
        void AddDisposable(IDisposable disposable);
    }
}