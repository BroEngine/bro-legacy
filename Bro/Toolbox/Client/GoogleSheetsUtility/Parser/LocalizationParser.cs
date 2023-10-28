using System.Collections.Generic;
using System.Linq;
using Bro.Json;
using Bro.Toolbox.Client;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class LocalizationParser : ISheetParser
    {
        public string SheetsToString(IEnumerable<(string, IList<IList<object>>)> sheets, string configTypeName, string targetSheetName = null)
        {
            var storageData = new Dictionary<string, Dictionary<string, string>>();

            foreach (var (sheetName, sheetData) in sheets)
            {
                var columnNames = sheetData[0].Cast<string>().ToList();
                for (int i = 1, max = columnNames.Count; i < max; i++)
                {
                    if (!storageData.TryGetValue(columnNames[i], out var config))
                    {
                        storageData.Add(columnNames[i], new Dictionary<string, string>());
                    }
                }
                
                for (int i = 1, max = sheetData.Count; i < max; i++)
                {
                    var row = sheetData[i];
                    var id = row[0];
                    var key = string.Format($"{sheetName}_{id}");
                    
                    for (int j = 1, subMax = row.Count; j < subMax; j++)
                    {
                        var language = columnNames[j].ToLower();

                        if (!storageData.ContainsKey(language))
                        {
                            storageData.Add(language, new Dictionary<string, string>());
                        }

                        storageData[language].Add($"[{key}]", row[j].ToString());
                    }
                }
            }

            if (storageData.TryGetValue(configTypeName, out var localizationConfig))
            {
                return JsonConvert.SerializeObject(localizationConfig);
            }
            else
            {
                Debug.LogError($"no language for type {configTypeName}");
                return string.Empty;
            }
        }

    }

}
