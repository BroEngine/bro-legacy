using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bro.Network.Tcp.Engine.Client;
using Bro.Toolbox;
using Bro.Toolbox.Client;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Bro.Sketch.Client
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Settings("AudioSettings", "Resources/Settings")]
    public class AudioSettings : SystemSettings<AudioSettings>
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private SerializableDictionary<string, AudioClipSetting> _clips = new SerializableDictionary<string, AudioClipSetting>();
        
        public AudioMixer Mixer => _mixer;
        
        private const string audioPath = "Assets/Resources/Audio/";

        public (string Path, float Volume) GetClipPathVolume(string id)
        {
            if (!_clips.ContainsKey(id))
            {
                Debug.LogError($"audio settings :: id \"{id}\" not found in {_clips.Count} elements.");
                return (null, -1);
            }

            return (_clips[id].Path, _clips[id].Volume);
        }
        
        public float GetClipVolume(string id)
        {
            if (!_clips.ContainsKey(id))
            {
                Debug.LogError($"audio settings :: id \"{id}\" not found in {_clips.Count} elements.");
                return -1f;
            }

            return _clips[id].Volume;
        }

        public IEnumerable<string> GetClipKeys(string substring)
        {
            return _clips.Where(c => c.Key.Contains(substring)).Select(p => p.Key);
        }
        
        public int GetClipsCount(string substring)
        {
            return _clips.Count(c => c.Key.Contains(substring));
        }

        public string GetRandomClipId(string substring)
        {
            var clips = _clips.Where(c => c.Key.Contains(substring));
            if (!clips.Any())
            {
                return String.Empty;
            }
            return clips.RandomItem().Key;
        }
        
#if UNITY_EDITOR
        [MenuItem("Settings/Audio Settings")]
        public static void Edit()
        {
            Instance = null;
            Selection.activeObject = Instance;
            DirtyEditor();
        }

        public void UpdatePaths()
        {
            foreach (var clipInfo in _clips)
            {
                clipInfo.Value.ClipPathSetup();
            }
        }
#endif
        
        [Serializable]
        public class AudioClipSetting
        {
#if UNITY_EDITOR
            [SerializeField] private UnityEngine.AudioClip _clip;
#endif
            [SerializeField, OnChanged("ClipPathSetup")] private string _path;
            [SerializeField, UnityEngine.Range(0, 1)] private float _volume = 1;
            
            public string Path => _path;
            public float Volume => _volume;
            
#if UNITY_EDITOR
            public void ClipPathSetup()
            {
                if (_clip == null)
                {
                    return;
                }

                _path = GetPath(_clip);
            }
            
            private static string GetPath(UnityEngine.Object file)
            {
                var fullPath = UnityEditor.AssetDatabase.GetAssetPath(file);
                return Regex.Replace(fullPath, @"^Assets/Resources/(.*?)(\.\w+)$", "$1");
            }
#endif
        }
    }
}