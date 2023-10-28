#if UNITY_EDITOR
using System.Collections.Generic;

namespace Bro.Toolbox.Client
{
    public static class GoogleSheetsLoader
    {
        public static IEnumerable<(string, IList<IList<object>>)> Load(string spreadsheetID, string credentials)
        {
            var googleSheetsReader = GoogleSheetsReader.Create(spreadsheetID, credentials);
            return googleSheetsReader.LoadAllSheets();
        }
    }
}
#endif