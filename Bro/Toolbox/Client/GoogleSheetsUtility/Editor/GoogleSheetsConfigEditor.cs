#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bro.Json;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(GoogleSheetsSettings))]
    public class GoogleSheetsConfigEditor : Editor
    {
        private GoogleSheetsSettings _settings;

        public override void OnInspectorGUI()
        {
            _settings = (GoogleSheetsSettings)target;

            if (_settings == null)
            {
                return;
            }

            if (_settings.Configs == null)
            {
                _settings.Configs = new List<GoogleSheetsSettings.ConfigStruct>();
            }

            if (_settings.Configs.Count == 0)
            {
                _settings.Configs.Add(new GoogleSheetsSettings.ConfigStruct());
            }


            GUILayout.Space(10);
            _settings.Credentials = EditorGUILayout.TextField(new GUIContent("Credentials", "Path to google service .json file. \nEx: Assets/google_service.json"), _settings.Credentials);
            if (GUILayout.Button("Open credentials", GUILayout.Width(150)))
            {
                string path = EditorUtility.OpenFilePanel("Open credentials file", "Assets/", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    path = FileUtil.GetProjectRelativePath(path);
                    _settings.Credentials = path;
                }
            }
            _settings.ConfigsPath = EditorGUILayout.TextField(new GUIContent("Config directory", "Path to configs directory"), _settings.ConfigsPath);
            if (GUILayout.Button("Select directory", GUILayout.Width(150)))
            {
                string path = EditorUtility.OpenFolderPanel("Open credentials file", "Assets/", "Configs");
                if (!string.IsNullOrEmpty(path))
                {
                    path = FileUtil.GetProjectRelativePath(path);
                    _settings.ConfigsPath = path;
                }
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Load all", GUILayout.Width(85)))
            {
                foreach (var config in _settings.Configs)
                {
                    LoadConfig(config);
                }
                Debug.Log($"Finished loading all configs");
            }

            GUILayout.Space(_settings.Configs.Count > 1 ? 0 : 19);

            foreach (var config in _settings.Configs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (_settings.Configs.Count > 1)
                {
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Confirmation",
                            $"Are you sure you want to delete {config.TypeName} config?", "Yes", "No"))
                        {
                            _settings.Configs.Remove(config);
                            break;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                config.TypeName = EditorGUILayout.TextField(new GUIContent("Type name", "Name of type to parse"), config.TypeName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                config.SheetName = EditorGUILayout.TextField(new GUIContent("Sheet name", "Sheet name in table"), config.SheetName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                config.ParserType = (ParserType)EditorGUILayout.EnumPopup(new GUIContent("Parser type"), config.ParserType);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                config.SpreadsheetID = EditorGUILayout.TextField(
                    new GUIContent("SpreadsheetID", "Spreadsheet ID from url"),
                    config.SpreadsheetID);

                if (!string.IsNullOrEmpty(config.SpreadsheetID))
                {
                    if (GUILayout.Button("Open sheet", GUILayout.Width(100)))
                    {
                        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{config.SpreadsheetID}");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (!string.IsNullOrEmpty(config.SpreadsheetID))
                {
                    if (GUILayout.Button("Load", GUILayout.Width(85)))
                    {
                        LoadConfig(config);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                if (_settings.Configs.Last() == config)
                {
                    if (GUILayout.Button("+", GUILayout.Width(25)))
                    {
                        _settings.Configs.Add(new GoogleSheetsSettings.ConfigStruct());
                        break;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
            }

            SerializedObject serializedSettings = new SerializedObject(_settings);
            serializedSettings.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_settings);
            }
        }

        private void LoadConfig(GoogleSheetsSettings.ConfigStruct config)
        {
            if (_settings.Credentials.Length == 0)
            {
                Debug.LogError("Credentials field is empty");
            }
            else if (!File.Exists(_settings.Credentials))
            {
                Debug.LogError("Credentials file does not exist: " + _settings.Credentials);
            }
            else
            {
                var sheets = GoogleSheetsLoader.Load(config.SpreadsheetID, _settings.Credentials);
                ConfigParser.Parse(sheets, config.SheetName, config.TypeName, _settings.ConfigsPath, config.ParserType);
                Debug.Log($"Finished loading config {config.TypeName}");
            }
        }
    }
}

#endif