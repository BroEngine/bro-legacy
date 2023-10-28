using Bro.Client;

namespace Bro.Sketch.Client
{
    public class LoadingInterruptionEvent : Event
    {
        public readonly LoadingInterruptionType Type;

        public LoadingInterruptionEvent(LoadingInterruptionType type)
        {
            Type = type;
        }
    }
}