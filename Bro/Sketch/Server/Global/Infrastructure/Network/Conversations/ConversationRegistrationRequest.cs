using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationRegistrationRequest : ServiceRequest<ConversationRegistrationRequest>
    {
        public readonly ArrayParam<IntParam> Users = new ArrayParam<IntParam>(short.MaxValue);
        public readonly IntParam LifeTime = new IntParam(); // seconds
        
        public ConversationRegistrationRequest() : base(Network.Request.OperationCode.Social.ConversationRegistration, new ConversationChannel() )
        {
            AddParam(Users);
            AddParam(LifeTime);
        }
    }
}