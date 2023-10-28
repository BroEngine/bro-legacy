using Bro.Client;

namespace Bro.Sketch.Client
{
    public class AuthorizationEvent : Event
    {
        public readonly bool IsAuthorized; 
        public AuthorizationEvent(bool isAuthorized)
        {
            IsAuthorized = isAuthorized;
        }
    }
}