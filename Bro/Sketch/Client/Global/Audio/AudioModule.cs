#if !CONSOLE_CLIENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bro.Client;
using Bro.Client.Context;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class AudioModule : IAudioManager, IClientContextModule
    {
        private IClientContext _context;
        private float _masterVolume;
        private AudioSettings _settings;
        private AudioPlayer _player;

        private const string _soundStorageKey = "master_volume";
        private const string _masterMixerName = "Master";
        private const string _musicMixerName = "Music";
        private const string _soundMixerName = "Sound";
        private const float _volumeThresholdMin = -80f;
        private const float _volumeThresholdMax = 0f;

        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        public bool IsMute => _masterVolume <= 0.001;
        
        void IClientContextModule.Initialize(IClientContext clientContext)
        {
            _context = clientContext;
            var objectPlayer = new GameObject("audio_player");
            UnityEngine.Object.DontDestroyOnLoad(objectPlayer);
            _player = objectPlayer.AddComponent<AudioPlayer>();
            _player.Init();

            // TODO : загрузка отдельного файла с настройками для каждого контекста, если понадобится
            //_settings = Resources.Load<Bro.Sketch.Client.AudioSettings>(_clientContext.GetType().ToString());
        }

        IEnumerator IClientContextModule.Load()
        {
            _context.AddDisposable(new EventObserver<PlayAudioEvent>(OnPlayAudioEvent));
            _context.AddDisposable(new EventObserver<StopAudioEvent>(OnStopAudioEvent));
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            return null;
        }

        private void OnPlayAudioEvent(PlayAudioEvent playAudioEvent)
        {
            switch (playAudioEvent.Type)
            {
                case SoundType.Sound:
                    var sound = PlaySound2D(playAudioEvent.SoundID, playAudioEvent.Type, _context);
                    if (sound != null)
                    {
                        sound.Source.loop = playAudioEvent.IsLoop;
                    }
                    break;
                case SoundType.Music:
                    PlayMusic(playAudioEvent.SoundID, playAudioEvent.Type, _context);
                    break;
            }
        }
        
        private void OnStopAudioEvent(StopAudioEvent stopAudioEvent)
        {
            switch (stopAudioEvent.Type)
            {
                case SoundType.Sound:
                    StopSound(stopAudioEvent.SoundID, _context);
                    break;
                case SoundType.Music:
                    StopMusic(_context);
                    break;
            }
        }

        /// <summary>
        /// Громкость всех звуков
        /// </summary>
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                // в AudioMixer значения громкости представлены в децибелах от -80 до 0
                _masterVolume = value;
                
                var vol = Mathf.Lerp(_volumeThresholdMin, _volumeThresholdMax, value);
                _player.Mixer?.SetFloat(_masterMixerName, vol);
            }
        }
        
        /// <summary>
        /// Громкость музыки
        /// </summary>
        public float MusicVolume
        {
            set
            {
                // в AudioMixer значения громкости представлены в децибелах от -80 до 0
                var vol = Mathf.Lerp(_volumeThresholdMin, _volumeThresholdMax, value);
                _player.Mixer?.SetFloat(_musicMixerName, vol);
            }
        }

        /// <summary>
        /// Громкость звуков
        /// </summary>
        public float SoundVolume
        {
            set
            {
                // в AudioMixer значения громкости представлены в децибелах от -80 до 0
                var vol = Mathf.Lerp(_volumeThresholdMin, _volumeThresholdMax, value);
                _player.Mixer?.SetFloat(_soundMixerName, vol);
            }
        }

        /// <summary>
        /// Возпроизвести звук
        /// </summary>
        public AudioClipInfo PlaySound2D(string id, SoundType type, IClientContext context, AudioSource source = null)
        {
            if (IsMute)
            {
                return null;
            }
            
            var clip = new AudioClipInfo(id, type, source != null);
            if (source == null)
            {
                _player.CreateSound(clip, FindListenerPosition());
            }
            else
            {
                _player.AddSound(clip, source);
            }
            clip.Source.spatialBlend = 0;
            if (clip.Source.clip != null)
            {
                StopSound(clip, context, clip.Source.clip.length);
            }

            return clip;
        }

        /// <summary>
        /// Воспроизвести звук в пространстве
        /// </summary>
        public AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, UnityEngine.Vector3 position)
        {
            if (IsMute)
            {
                return null;
            }
            
            var clip = new AudioClipInfo(id, type);
            _player.CreateSound(clip, position);
            clip.Source.spatialBlend = 0.5f;
            StopSound(clip, context, clip.Source.clip.length);

            return clip;
        }

        /// <summary>
        /// Воспроизвести звук в пространстве
        /// </summary>
        public AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, AudioSource source)
        {
            if (IsMute)
            {
                return null;
            }
            
            var clip = new AudioClipInfo(id, type, source != null);
            _player.AddSound(clip, source);
            clip.Source.spatialBlend = 0.5f;
            StopSound(clip, context, clip.Source.clip.length);

            return clip;
        }

        /// <summary>
        /// Переместить источник звука
        /// </summary>
        public void MoveSoundSource(AudioClipInfo clip, UnityEngine.Vector3 newPosition)
        {
            clip.Source.transform.position = newPosition;
        }

        /// <summary>
        /// Остановить звук
        /// </summary>
        public void StopSound(AudioClipInfo clip, IClientContext context, float delay = 0)
        {
            context.Scheduler.Schedule(() => _player.RemoveSound(clip), delay);
        }
        
        public void StopSound(string clipId, IClientContext context, float delay = 0)
        {
            context.Scheduler.Schedule(() => _player.RemoveSound(clipId), delay);
        }

        /// <summary>
        /// Воспроизвести музыку
        /// </summary>
        public void PlayMusic(string id, SoundType type, IClientContext context)
        {
            var clip = new AudioClipInfo(id, type);

            _player.AddMusic(clip);
        }

        /// <summary>
        /// Поставить музыку на паузу
        /// </summary>
        public void PauseMusic(IClientContext context)
        {
            _player.MusicClipInfo?.Source.Pause();
        }

        /// <summary>
        /// Продолжить воспроизведение музыки
        /// </summary>
        public void ResumeMusic(IClientContext context)
        {
            _player.MusicClipInfo?.Source.UnPause();
        }

        /// <summary>
        /// Остановить музыку
        /// </summary>
        public void StopMusic(IClientContext context)
        {
            _player.MusicClipInfo.Source.Stop();
        }

        private UnityEngine.Vector3 FindListenerPosition()
        {
            var listeners = new List<AudioListener>();
            listeners.AddRange(GameObject.FindObjectsOfType<AudioListener>());

            return listeners.FirstOrDefault(l => l.enabled).transform.position;
        }
    }
}
#endif