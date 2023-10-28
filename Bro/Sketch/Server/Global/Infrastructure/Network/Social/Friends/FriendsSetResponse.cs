using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class FriendsSetResponse : ServiceResponse<FriendsSetResponse>
    {
        public readonly ByteParam Result = new ByteParam();
        
        public FriendsSetResponse() : base(Request.OperationCode.Social.FriendsSet, new ProfileChannel() )
        {
            AddParam(Result);
        }
    }
}