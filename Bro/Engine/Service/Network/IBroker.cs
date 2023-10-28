using System;
using Bro.Network.Service;
using Bro.Service.Context;

namespace Bro.Service
{
    public interface IBroker
    {
        bool IsStarted { get; }
        bool IsConnected { get; }

        void Start(BrokerConfig brokerConfig);
        void RegisterDispatcher(IServiceDispatcher dispatcher);
        void RemoveDispatcher(IServiceDispatcher dispatcher);
        void Send(IServiceOperation operation);
        void SendRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback);
        void SendRequest(IServiceRequest serviceRequest);

        IServiceChannel PrivateChannel { get; }
    }
}