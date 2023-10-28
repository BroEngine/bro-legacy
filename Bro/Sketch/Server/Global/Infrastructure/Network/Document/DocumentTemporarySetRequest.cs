using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentTemporarySetRequest : ServiceRequest<DocumentTemporarySetRequest>
    {
        public readonly StringParam Data = new StringParam();
        
        public DocumentTemporarySetRequest() : base(Network.Request.OperationCode.Infrastructure.DocumentTemporarySet, new DocumentChannel() )
        {
            AddParam(Data);
        }
    }
}