using Bro.Json;
using Bro.Network.Tcp.Engine.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class MultipleConfigParser : ISheetParser
    {
        [Serializable]
        private class ConfigModel
        {
            [JsonProperty("default_config")]public string DefaultConfig;
            [JsonProperty("data")] public Dictionary<string, object> Data = new Dictionary<string, object>();
        }

        private string TableToString(IList<IList<object>> table, string typeName)
        {
            bool hasCustomType = false;
            var type = ConfigParser.GetTargetType(typeName);
            if (type == null)
            {
                return null;
            }

            for (int i = 1; i < table.Count; i++) // adding empty column to first row for dynamic types
            {
                if (table[i].Count > table[0].Count)
                {
                    for (int j = 0, jMax = table[i].Count - table[0].Count; j < jMax; j++)
                    {
                        table[0].Add(new Tuple<FieldInfo, int>(null, table[0].Count));
                    }
                }
            }

            // search for fields from table
            if ((string) table[0][1] == "__type")
            {
                hasCustomType = true;
            }

            var fields = GetFields(table, type);

            var dict = new Dictionary<string, object>();
            string defaultKey = null;
            for (int i = 1; i < table.Count; i++)
            {
                var itemProperties = new Dictionary<string, object>();
                if (hasCustomType)
                {
                    var customTypeString = (string) table[i][1];
                    var customType = ConfigParser.GetTargetType(customTypeString);
                    fields = GetFields(table, customType);
                    itemProperties.Add("__type",customTypeString);
                }
                
                foreach (var field in fields)
                {
                    if (table[i].Count <= field.Item2)
                    {
                        continue;
                    }
                    if (field.Item1 == null) // add type that not represented in table's first row
                    {
                        var item = table[i][field.Item2];
                        if ((item as string).IsNullOrEmpty())
                        {
                            continue;
                        }

                        var parts = ((string)item).Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2) // wrong format. must be [type_name]:[value]
                        {
                            Debug.LogError($"Wrong format for dynamic type in row {i} column {field.Item1.Name}");
                            continue;
                        }
                        foreach (var parser in ConfigParser.TypeParsers)
                        {
                            var fieldType = field?.Item1?.FieldType == null ? parser.Value : field.Item1.FieldType;
                            var result = parser.Key.Convert(parts[1], fieldType);
                            if (result != null)
                            {
                                itemProperties.Add(parts[0], result);
                                break;
                            }
                        }
                    }
                    else // add type that is in first row
                    {
                        foreach (var parser in ConfigParser.TypeParsers.Keys)
                        {
                            var result = parser.Convert(table[i][field.Item2], field.Item1.FieldType);
                            if (result != null)
                            {
                                var fieldCustomAttributes = field.Item1.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
                                var fieldAttribute = (JsonPropertyAttribute)fieldCustomAttributes[0];
                                var fieldAttributeName = fieldAttribute.PropertyName;
                                itemProperties.Add(fieldAttributeName, result);
                                break;
                            }
                        }
                    }
                }

                // first column in the table contains the item keys
                var key = (string)table[i][0];
                if (dict.ContainsKey(key))
                {
                    Debug.LogError($"Table allready contains item {key}");
                }
                else
                {
                    dict.Add(key, itemProperties);
                    if (defaultKey == null)
                    {
                        defaultKey = key;
                    }
                }
            }

            var model = new ConfigModel
            {
                DefaultConfig = defaultKey,
                Data = dict
            };

            var data = JsonConvert.SerializeObject(model, Formatting.Indented);
            return data;

        }

   
        private static List<Tuple<FieldInfo, int>> GetFields(IList<IList<object>> table, Type type)
        {
            var fields = new List<Tuple<FieldInfo, int>>();
            var fieldInfoArray = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfoArray) // add fields that type has
            {
                var fieldCustomAttributes = fieldInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
                if (fieldCustomAttributes.Length == 0)
                {
                    continue;
                }

                var fieldAttribute = (JsonPropertyAttribute) fieldCustomAttributes[0];
                var fieldAttributeName = fieldAttribute.PropertyName;
                fieldAttributeName = Regex.Replace(fieldAttributeName, @"(\[|\])", string.Empty);

                for (int i = 0; i < table[0].Count; i++)
                {
                    if (fieldAttributeName.Equals(table[0][i] as string, StringComparison.CurrentCultureIgnoreCase))
                    {
                        fields.Add(Tuple.Create(fieldInfo, i));
                    }
                }
            }

            for (int i = 1; i < table[0].Count; i++) // add additional fields
            {
                if ((table[0][i] as string).IsNullOrEmpty())
                {
                    fields.Add(Tuple.Create<FieldInfo, int>(null, i));
                }
            }

            return fields;
        }

        public string SheetsToString(IEnumerable<(string, IList<IList<object>>)> sheets, string configTypeName, string sheetName = null)
        {
            if (sheetName == null)
            {
                Debug.LogError("multiple parser :: sheetName is null");
                return null;
            }
            foreach (var sheet in sheets)
            {
                if (sheet.Item1.Equals(sheetName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return TableToString(sheet.Item2, configTypeName);
                }
            }
            Debug.LogError($"multiple parser :: {sheetName} not found");
            return null;
        }
    }
}