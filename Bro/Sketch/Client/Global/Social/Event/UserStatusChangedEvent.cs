using Bro.Client;

namespace Bro.Sketch.Client
{
    public class UserStatusChangedEvent : Event
    {
        public readonly UserStatus Status;
        
        public UserStatusChangedEvent(UserStatus status)
        {
            Status = status;
        }
    }
}