namespace Bro.Sketch.Client
{
    public class PlayAudioEvent : Bro.Client.Event
    {
        public readonly string SoundID;
        public readonly SoundType Type;
        public readonly bool IsLoop;
        
        public PlayAudioEvent(string soundId, SoundType type, bool isLoop = false)
        {
            SoundID = soundId;
            Type = type;
            IsLoop = isLoop;
        }
    }
}