using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class UserStatusEvent : ServiceEvent<UserStatusEvent>
    {
        public readonly IntParam DestinationUserId = new IntParam();
        public readonly UserStatusParam Status = new UserStatusParam();   
        
        public UserStatusEvent() : base(Event.OperationCode.Social.UserStatus, null )
        {
            AddParam(DestinationUserId);
            AddParam(Status);
        }
    }
}