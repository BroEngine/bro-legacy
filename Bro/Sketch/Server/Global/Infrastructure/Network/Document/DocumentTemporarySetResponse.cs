using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentTemporarySetResponse : ServiceResponse<DocumentTemporarySetResponse>
    {
        public readonly StringParam Token = new StringParam();
        
        public DocumentTemporarySetResponse() : base(Network.Request.OperationCode.Infrastructure.DocumentTemporarySet, new DocumentChannel())
        {
            AddParam(Token);
        }
    }
}