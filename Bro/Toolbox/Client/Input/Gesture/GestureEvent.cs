namespace Bro.Toolbox.Client
{
    public class GestureEvent : Bro.Client.Event
    {
        public readonly Gesture.Gesture Gesture;

        public GestureEvent(Gesture.Gesture gesture)
        {
            Gesture = gesture;
        }
    }
}