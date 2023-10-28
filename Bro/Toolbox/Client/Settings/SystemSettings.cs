using UnityEngine;
using System.IO;
using System;

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
namespace Bro.Toolbox.Client
{
    public class SystemSettings<T> : ScriptableObject where T : ScriptableObject
    {
        public static string Name
        {
            get
            {
                if (Attribute.IsDefined(typeof(T), typeof(SettingsAttribute)))
                {
                    var attributeValue = Attribute.GetCustomAttribute(typeof(T), typeof(SettingsAttribute)) as SettingsAttribute;
                    if (!string.IsNullOrEmpty(attributeValue.Name))
                    {
                        return attributeValue.Name;
                    }
                }

                return typeof(T).Name;
            }
        }

        public static string SavingPath
        {
            get
            {
                if (Attribute.IsDefined(typeof(T), typeof(SettingsAttribute)))
                {
                    var attributeValue = Attribute.GetCustomAttribute(typeof(T), typeof(SettingsAttribute)) as SettingsAttribute;
                    if (!string.IsNullOrEmpty(attributeValue.Path))
                    {
                        return Path.Combine("Assets", attributeValue.Path);
                    }
                }

                return "Assets/Resources";
            }
        }

        public static string LoadingPath
        {
            get
            {
                var savingPath = SavingPath;
                if (!savingPath.Contains("Resources"))
                {
                    Log.Error("setting file :: settings class " + Name + " not contain resource folder in path");
                    return string.Empty;
                }

                var splits = savingPath.Split(new string[] {"Resources"}, StringSplitOptions.None);
                return splits[splits.Length - 1].TrimStart('/');
            }
        }

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    LoadInstanceForResources();

#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        Log.Info("settings file :: resource file not found, type = " + typeof(T) + ", creating asset");
                        CreateSettingsAsset();

                        LoadInstanceForResources();

                        if (_instance == null)
                        {
                            Log.Error("settings file :: reload error, type = " + typeof(T) + ", looks like creation failed");
                        }
                    }
#endif
                }

                return _instance;
            }
            set { _instance = value; }
        }

        private static void LoadInstanceForResources()
        {
            _instance = null;

            try
            {
                _instance = Resources.Load(ValidatePath(Path.Combine(LoadingPath, Name))) as T;
            }
            catch (Exception ex)
            {
                Log.Error("settings file :: type = " + typeof(T) + ", loading error = " + ex.Message);
            }
        }

        private static string ValidatePath(string path)
        {
#if UNITY_STANDALONE_WIN
            return path.Replace("\\","/");
#endif
            return path;
        }

#if UNITY_EDITOR
        private static void CreateSettingsAsset()
        {
            _instance = CreateInstance<T>();

            if (_instance == null)
            {
                
                UnityEngine.Debug.LogError("failed to create instance of type = " + typeof(T) + " " + _instance + " name = " + Name);
            }

            var savingPath = SavingPath;
            if (!Directory.Exists(savingPath))
            {
                Directory.CreateDirectory(savingPath);
            }

            var fullPath = ValidatePath(Path.Combine(savingPath, Name + ".asset"));
            UnityEditor.AssetDatabase.CreateAsset(_instance, fullPath);
        }

        protected static void DirtyEditor()
        {
            UnityEditor.EditorUtility.SetDirty(Instance);
        }
#endif
    }
}
#else
public class SystemSettings<T> : ScriptableObject where T : ScriptableObject
{
    public static T Instance => default(T);
}
#endif