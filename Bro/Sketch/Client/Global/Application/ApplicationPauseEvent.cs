namespace Bro.Sketch.Client
{
    public class ApplicationPauseEvent : Bro.Client.Event
    {
        public readonly bool IsPause;

        public ApplicationPauseEvent(bool isPause)
        {
            IsPause = isPause;
        }
    }
}