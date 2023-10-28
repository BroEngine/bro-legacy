using System.Collections.Generic;

namespace Bro
{
    public interface IDataTable : IEnumerable<KeyValuePair<object, object>>
    {
        object this[object key] { get; set; }
        bool Exist(object key);
        IDataTable Clone();
    }
}