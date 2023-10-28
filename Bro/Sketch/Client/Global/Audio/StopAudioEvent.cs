namespace Bro.Sketch.Client
{
    public class StopAudioEvent : Bro.Client.Event
    {
        public readonly string SoundID;
        public readonly SoundType Type;
        
        public StopAudioEvent(string soundId, SoundType type)
        {
            SoundID = soundId;
            Type = type;
        }
    }
}