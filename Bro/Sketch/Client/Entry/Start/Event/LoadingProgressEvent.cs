using Bro.Client;

namespace Bro.Sketch.Client
{
    public class LoadingProgressEvent : Event
    {
        public readonly float Value;
        
        public LoadingProgressEvent(float value)
        {
            Value = value;
        }
    }
}