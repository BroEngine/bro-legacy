using Bro.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public static class ConfigParser
    {
        /// parsers for different types
        public static readonly Dictionary<ITypeParser, Type> TypeParsers = new Dictionary<ITypeParser, Type>
        {
            { new ListTypeParser(), typeof(IList<object>) },
            { new Vector2Parser(), typeof(Vector2)},
            { new Vector3Parser(), typeof(Vector3)},
            { new EnumTypeParser(), typeof(Enum) },
            { new FloatTypeParser(), typeof(float) },
            { new BoolTypeParser(), typeof(bool) },
            { new StringTypeParser(), typeof(string) },
            { new BaseTypeParser(), typeof(object) },


        };

        /// save data for certain type to json file 
        public static void Parse(IEnumerable<(string, IList<IList<object>>)> sheets, string sheetName, string configTypeName, string configPath, ParserType parserType)
        {
            var parserInstance = GetParserType(parserType);
            if (parserInstance != null)
            {
                var config = parserInstance.SheetsToString(sheets, configTypeName, sheetName);
                if (config != null)
                {
                    var configFileName = configTypeName.Replace("_config", string.Empty);
                    SaveToFile(config, configPath, configFileName);
                }
            }
            else
            {
                Debug.LogError($"Parser {parserType} not found");
            }
            
        }

        /// find type in current assembly by name
        public static Type GetTargetType(string typeName)
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigParser));

            foreach (var type in assembly.GetTypes())
            {
                var typeCustomAttributes = type.GetCustomAttributes(typeof(JsonTypeAttribute), true);
                if (typeCustomAttributes.Length == 0)
                {
                    continue;
                }
                var typeAttribute = (JsonTypeAttribute)typeCustomAttributes[0];
                var typeAttributeName = typeAttribute.TypeName;
                typeAttributeName = Regex.Replace(typeAttributeName, @"(\[|\])", string.Empty);
                if (typeAttributeName.Equals(typeName))
                {
                    return type;
                }
            }



            //var type = assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase));

            Debug.LogError($"Type {typeName} not found in current assembly {assembly.FullName}");
            return null;
        }

        /// save string to file
        private static void SaveToFile(string config, string path, string name)
        {
            var filePath = Path.Combine(Application.dataPath, path.Replace("Assets/", ""), $"{name}.json");
            File.WriteAllText(filePath, config);
        }

        private static ISheetParser GetParserType(ParserType parserType) =>
            parserType switch
            {
                ParserType.MultipleItems => new MultipleConfigParser(),
                ParserType.SingleItem => new SingleConfigParser(),
                ParserType.Localization => new LocalizationParser(),
                _ => null,
            };
    }
}