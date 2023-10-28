using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    /* internal */
    public class DocumentCumulativeGetRequest : ServiceRequest<DocumentCumulativeGetRequest>
    {
        public readonly StringParam Token = new StringParam();
        
        public DocumentCumulativeGetRequest() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeGet, new DocumentChannel() )
        {
            AddParam(Token);
        }
    }
}