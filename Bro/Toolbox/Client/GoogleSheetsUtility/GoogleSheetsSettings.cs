#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Bro.Toolbox.Client
{
    [InitializeOnLoad]
    [Settings("GoogleSheetsSettings", "Resources/Settings")]
    public class GoogleSheetsSettings : SystemSettings<GoogleSheetsSettings>
    {
        [MenuItem("Settings/GoogleSheets Settings")]
        private static void OpenSettings()
        {
            Instance = null;
            Selection.activeObject = Instance;
            DirtyEditor();
        }

        [Serializable]
        public class ConfigStruct
        {
            public string TypeName;
            public string SheetName;
            public string SpreadsheetID;
            public ParserType ParserType;
        }

        public List<ConfigStruct> Configs;

        public string Credentials;
        public string ConfigsPath;
    }
}
#endif