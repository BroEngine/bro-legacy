using System;
using Bro.Network.Service;
using Bro.Service;

namespace Bro.Service.Context
{
    public interface IServiceDispatcher
    {
        void HandleServiceEvent(IServiceEvent serviceEvent);
        void HandleServiceRequest(IServiceRequest serviceRequest);
        void SendServiceEvent(IServiceEvent serviceEvent);
        void SendServiceRequest(IServiceRequest serviceRequest, Action<IServiceResponse> callback);
        bool IsServiceUsable { get; }
        IServiceChannel PrivateChannel { get; }
    }
}