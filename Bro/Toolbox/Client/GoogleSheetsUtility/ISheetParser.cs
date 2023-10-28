using System.Collections.Generic;

namespace Bro.Toolbox.Client
{
    public interface ISheetParser
    {
        string SheetsToString(IEnumerable<(string, IList<IList<object>>)> sheets, string configTypeName, string sheetName=null);
    }
}