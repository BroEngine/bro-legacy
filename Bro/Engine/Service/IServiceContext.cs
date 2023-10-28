using Bro.Network;

namespace Bro.Service.Context
{
    public interface IServiceContext 
    {
        void Initialize();
        void Start();
        void Stop();

        void HandleRequest(INetworkRequest request);

        T GetComponent<T>() where T : class, IServiceContextComponent;
        
        IScheduler Scheduler { get; }
    }
}