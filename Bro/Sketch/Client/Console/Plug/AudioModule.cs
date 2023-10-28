#if CONSOLE_CLIENT
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class AudioModule : IAudioManager, IClientContextModule
    {
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }
        public AudioClipInfo PlaySound2D(string id, SoundType type, IClientContext context, AudioSource source = null)
        {
            throw new System.NotImplementedException();
        }

        public AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, AudioSource source)
        {
            throw new System.NotImplementedException();
        }

        public void MoveSoundSource(AudioClipInfo clip, Vector3 newPosition)
        {
            throw new System.NotImplementedException();
        }

        public void StopSound(AudioClipInfo clip, IClientContext context, float delay = 0)
        {
            throw new System.NotImplementedException();
        }

        public void PlayMusic(string id, SoundType type, IClientContext context)
        {
            throw new System.NotImplementedException();
        }

        public void PauseMusic(IClientContext context)
        {
            throw new System.NotImplementedException();
        }

        public void ResumeMusic(IClientContext context)
        {
            throw new System.NotImplementedException();
        }

        public void StopMusic(IClientContext context)
        {
            throw new System.NotImplementedException();
        }

        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers { get; }
        public void Initialize(IClientContext context)
        {
        }

        public IEnumerator Load()
        {
            return null;
        }

        public IEnumerator Unload()
        {
            return null;
        }
    }
}
#endif