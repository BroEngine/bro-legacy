using System.Collections.Generic;

namespace Bro.Server.Context
{
    public interface IServerContextModule
    {
        IList<CustomHandlerDispatcher.HandlerInfo> Handlers { get; }
        void Initialize(IServerContext context);
    }
}