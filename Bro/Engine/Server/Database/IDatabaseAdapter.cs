using System;
using System.Collections.Generic;

namespace Bro.Server.Database
{
    public interface IDatabaseAdapter
    {
        void Execute(IDatabaseRequest request, Action onComplete);
        void ExecuteQuery<T>(IDatabaseRequest request, Action<List<T>> onComplete) where T : class, new();
        void ExecuteQueries<T>(IDatabaseRequest request, Action<List<List<T>>> onComplete) where T : class, new();
        void ExecureProcedure(IDatabaseRequest request, Action<bool> onComplete);
        void ExecureProcedures(IDatabaseRequest request, Action<List<bool>> onComplete);
    }
}