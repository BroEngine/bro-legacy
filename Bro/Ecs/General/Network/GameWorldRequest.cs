using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Ecs
{
    public class GameWorldRequest : Bro.Network.NetworkRequest<GameWorldRequest>
    {
        public readonly IntParam FrameApplied = new IntParam(isOptional:true);
        public readonly BoolParam SharedApplied = new BoolParam(isOptional:true);
        
        public GameWorldRequest() : base(Request.OperationCode.EcsWorld)
        {
            AddParam(FrameApplied);
            AddParam(SharedApplied);
        }
    }
}