namespace Bro.Sketch.Client
{
    public class ApplicationFocusEvent : Bro.Client.Event
    {
        public readonly bool IsFocus;
   
        public ApplicationFocusEvent(bool isFocus)
        {
            IsFocus = isFocus;
        }
    }
}