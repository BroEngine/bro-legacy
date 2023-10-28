using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentCumulativeDeleteRequest : ServiceRequest<DocumentCumulativeDeleteRequest>
    {
        public readonly StringParam Token = new StringParam();
        
        public DocumentCumulativeDeleteRequest() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeDelete, new DocumentChannel() )
        {
            AddParam(Token);
        }
    }
}