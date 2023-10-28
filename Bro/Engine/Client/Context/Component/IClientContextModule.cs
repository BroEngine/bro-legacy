using System.Collections;
using System.Collections.Generic;
 
namespace Bro.Client
{
    public interface IClientContextModule
    {
        IList<CustomHandlerDispatcher.HandlerInfo> Handlers { get; }
        void Initialize(IClientContext context);
        IEnumerator Load();
        IEnumerator Unload();
    }
}