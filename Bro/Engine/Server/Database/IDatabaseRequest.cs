using System.Collections.Generic;

namespace Bro.Server.Database
{
    public class DatabaseProcedure
    {
        public string Name;
        public List<string> Parameters;
    }

    public interface IDatabaseRequest
    {
        List<string> Queries { get; }
        List<DatabaseProcedure> Procedures { get; }
    }
}