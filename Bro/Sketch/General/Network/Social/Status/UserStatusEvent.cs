using Bro.Network;

namespace Bro.Sketch.Network
{
    public class UserStatusEvent : NetworkEvent<UserStatusEvent>
    {
        public readonly UserStatusParam Status = new UserStatusParam();   
        
        public UserStatusEvent() : base(Event.OperationCode.Social.UserStatus)
        {
            AddParam(Status);
        }
    }
}