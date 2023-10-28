using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsDeleteResponse : ServiceResponse<FriendsDeleteResponse>
    {
        public readonly ByteParam Result = new ByteParam();
        
        public FriendsDeleteResponse() : base(Request.OperationCode.Social.FriendsDelete, new ProfileChannel() )
        {
            AddParam(Result);
        }
    }
}