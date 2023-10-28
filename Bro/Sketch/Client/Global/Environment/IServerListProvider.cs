using System;
using System.Collections.Generic;

namespace Bro.Sketch.Client
{
    public interface IServerListProvider
    {
        void GetServerList(Action<List<IConnectionConfig>> result);
    }
}