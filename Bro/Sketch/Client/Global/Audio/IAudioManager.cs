using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public interface IAudioManager
    {
        float MusicVolume { set; }
        float SoundVolume { set; }
        AudioClipInfo PlaySound2D(string id, SoundType type, IClientContext context, AudioSource source = null);
        AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, UnityEngine.Vector3 position);
        AudioClipInfo PlaySound3D(string id, SoundType type, IClientContext context, AudioSource source);
        void MoveSoundSource(AudioClipInfo clip, UnityEngine.Vector3 newPosition);
        void StopSound(AudioClipInfo clip, IClientContext context, float delay = 0);
        void PlayMusic(string id, SoundType type, IClientContext context);
        void PauseMusic(IClientContext context);
        void ResumeMusic(IClientContext context);
        void StopMusic(IClientContext context);
        
    }
}