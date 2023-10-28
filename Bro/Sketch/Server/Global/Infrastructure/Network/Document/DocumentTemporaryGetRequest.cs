using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentTemporaryGetRequest : ServiceRequest<DocumentTemporaryGetRequest>
    {
        public readonly StringParam Token = new StringParam();
        
        public DocumentTemporaryGetRequest() : base(Network.Request.OperationCode.Infrastructure.DocumentTemporaryGet, new DocumentChannel() )
        {
            AddParam(Token);   
        }
    }
}