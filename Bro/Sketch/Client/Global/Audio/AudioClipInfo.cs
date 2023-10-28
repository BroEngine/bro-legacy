using UnityEngine;

namespace Bro.Sketch.Client
{
    public class AudioClipInfo
    {
        public readonly string Id;
        public readonly SoundType Type;
        public readonly bool IsAttached;

        private AudioSource _source;
        public AudioSource Source
        {
            get => _source;
            set
            {
                _source = value;
                Poolable = _source.GetComponent<AudioSourcePoolable>();
            }
        }
        
        public AudioSourcePoolable Poolable { get; private set; }
        
        public AudioClipInfo(string id, SoundType type, bool isAttached = false)
        {
            Id = id;
            Type = type;
            IsAttached = isAttached;
        }

        public void SetVolume(float volume)
        {
            if (Source != null)
            {
                Source.volume = volume;
            }
        }
    }
}