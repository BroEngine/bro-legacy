using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationActionRequest : ServiceRequest<ConversationActionRequest>
    {
        public readonly LongParam ConversationId = new LongParam();
        public readonly IntParam UserId = new IntParam();
        public readonly ByteParam Action = new ByteParam();
        public readonly StringParam Title = new StringParam();
        public readonly StringParam Meta = new StringParam();
        
        public ConversationActionRequest() : base(Network.Request.OperationCode.Social.ConversationAction, new ConversationChannel() )
        {
            AddParam(ConversationId);
            AddParam(UserId);
            AddParam(Action);
            AddParam(Title);
            AddParam(Meta);
        }
    }
}