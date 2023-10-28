
using UnityEngine;
using System;
using Bro.Json;

namespace Bro.Toolbox.Client
{
    public class UserSettings<T> : ScriptableObject where T : ScriptableObject, IUserSettings, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Load();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static string Key
        {
            get { return "_user_settings_" + typeof(T); }
        }

        private static void Load()
        {
            _instance = null;
            var json = PlayerPrefs.GetString(Key);

            try
            {
                _instance = JsonConvert.DeserializeObject<T>(json);
                _instance.Validate();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("settings file :: type = " + typeof(T) + ", loading error = " + ex.Message);
            }

            if (_instance == null)
            {
                _instance = CreateInstance<T>();
                _instance.Validate();
                Save();
            }
        }

        private static void Save()
        {
            if (_instance != null)
            {
                var json = JsonConvert.SerializeObject(_instance);
                PlayerPrefs.SetString(Key, json);
                PlayerPrefs.Save();
            }
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            Save();
        }

        protected static void DirtyEditor()
        {
            Save();
        }
        #endif
    }
}

