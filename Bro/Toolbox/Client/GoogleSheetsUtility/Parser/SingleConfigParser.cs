using Bro.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class SingleConfigParser : ISheetParser
    {
        public string SheetsToString(IEnumerable<(string, IList<IList<object>>)> sheets, string configTypeName, string sheetName = null)
        {
            if (sheetName == null)
            {
                Debug.LogError("single parser :: sheetName is null");
                return null;
            }
            foreach (var sheet in sheets)
            {
                if (sheet.Item1.Equals(sheetName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return TableToString(sheet.Item2, configTypeName);
                }
            }
            Debug.LogError($"single parser :: {sheetName} not found");
            return null;
        }
        
        private string TableToString(IList<IList<object>> table, string typeName)
        {
            var type = ConfigParser.GetTargetType(typeName);
            if (type == null)
            {
                return null;
            }

            // search for fields from table
            var fieldInfoArray = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var newElement = Activator.CreateInstance(type);
            foreach (var fieldInfo in fieldInfoArray)
            {
                var fieldCustomAttributes = fieldInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
                if (fieldCustomAttributes.Length == 0)
                {
                    continue;
                }
                var fieldAttribute = (JsonPropertyAttribute)fieldCustomAttributes[0];
                var fieldAttributeName = fieldAttribute.PropertyName;
                fieldAttributeName = Regex.Replace(fieldAttributeName, @"(\[|\])", string.Empty);

                for (int i = 0; i < table[0].Count; i++)
                {
                    if (fieldAttributeName.Equals((string)table[0][i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var parser in ConfigParser.TypeParsers.Keys)
                        {
                            var result = parser.Convert(table[i][1], fieldInfo.FieldType);
                            if (result != null)
                            {
                                try
                                {
                                    fieldInfo.SetValue(newElement, result);
                                }
                                catch (Exception)
                                {
                                    Debug.LogError($"Error in parsing row {i} column {fieldInfo.Name}");
                                    throw;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return Json.JsonConvert.SerializeObject(newElement);
        }
    }
}
