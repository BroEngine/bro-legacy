using System;
using System.Collections.Generic;
using System.Linq;
using Bro.Network.Tcp.Engine.Client;
using Bro.Toolbox.Client;
using Bro.Toolbox.Client.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace Bro.Sketch.Client
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        
        private List<AudioClipInfo> _soundClips;
        private GameObjectPool<AudioSourcePoolable> _audioSourcePool;

    
        
        public AudioMixer Mixer { get; private set; }
        public AudioClipInfo MusicClipInfo { get; private set; }
        public IEnumerable<AudioClipInfo> AllSounds => _soundClips;

        public void Init()
        {
            return;
            // _soundClips = new List<AudioClipInfo>();
            // Mixer = AudioSettings.Instance.Mixer;
            // var prefab = UIAssetRegistry.Instance.GetElement<AudioSourcePoolable>();
            // _audioSourcePool = new GameObjectPool<AudioSourcePoolable>(prefab.gameObject, 64);
        }

        public void AddMusic(AudioClipInfo clipInfo)
        {
            if (_musicSource == null)
            {
                 var poolable = _audioSourcePool.Acquire();
                 _musicSource = poolable.AudioSource;
                
                 poolable.transform.parent = transform;
                _musicSource.outputAudioMixerGroup = Mixer.FindMatchingGroups(SoundType.Music.ToString()).FirstOrDefault();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            if (MusicClipInfo != null && MusicClipInfo.Id.Equals(clipInfo.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            var loadedAudio = LoadAudio(clipInfo.Id);
            MusicClipInfo = clipInfo;
            MusicClipInfo.Source = _musicSource;
            MusicClipInfo.Source.clip = loadedAudio.Clip;
            MusicClipInfo.SetVolume(loadedAudio.Volume);
            MusicClipInfo.Source.Play();
        }

        public void AddSound(AudioClipInfo clipInfo, AudioSource source)
        {
            var loadedAudio = LoadAudio(clipInfo.Id);
            clipInfo.Source = source;
            clipInfo.Source.clip = loadedAudio.Clip;
            clipInfo.SetVolume(loadedAudio.Volume);
            SetupAudioClip(clipInfo);
            _soundClips.Add(clipInfo);
            clipInfo.Source.Play();
        }
        
        public void CreateSound(AudioClipInfo clipInfo, UnityEngine.Vector3 targetPosition)
        {
            var poolable = _audioSourcePool.Acquire();
            var go = poolable.gameObject;
            go.transform.parent = transform;
            go.transform.position = targetPosition;
            AddSound(clipInfo, poolable.AudioSource);
        }

        public void RemoveSound(AudioClipInfo clipInfo)
        {
            if (clipInfo.IsAttached)
            {
                clipInfo.Source.Stop();
            }
            else
            {
                _audioSourcePool.Release(clipInfo.Poolable);
            }
            _soundClips.Remove(clipInfo);
        }

        public void RemoveSound(string clipId)
        {
            foreach (var clip in _soundClips.Where(c => c.Id.Equals(clipId, StringComparison.CurrentCultureIgnoreCase)))
            {
                if (clip.IsAttached)
                {
                    clip.Source.Stop();
                }
                else
                {
                    _audioSourcePool.Release(clip.Poolable);
                }
            }

            _soundClips.RemoveAll(c => c.Id.Equals(clipId, StringComparison.CurrentCultureIgnoreCase));
        }
        
        private (AudioClip Clip, float Volume) LoadAudio(string id)
        {
            var clipSettings = AudioSettings.Instance.GetClipPathVolume(id);
            if (clipSettings.Path.IsNullOrEmpty() || clipSettings.Volume <= 0)
            {
                return (null, clipSettings.Volume);
            }
            var clip = Resources.Load<AudioClip>(clipSettings.Path);
            return (clip, clipSettings.Volume);
        }

        private void SetupAudioClip(AudioClipInfo clipInfo)
        {
            clipInfo.Source.outputAudioMixerGroup = Mixer.FindMatchingGroups(clipInfo.Type.ToString()).FirstOrDefault();
            clipInfo.Source.loop = false;
            clipInfo.Source.playOnAwake = false;
        }
    }
}