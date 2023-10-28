using Bro.Fossil;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Ecs
{
    public class GameWorldEvent : Bro.Network.NetworkEvent<GameWorldEvent>
    {
        public readonly IntParam ReferenceFrame = new IntParam();
        public readonly IntParam Frame = new IntParam();
        public readonly BufferedByteArrayParam Data = new BufferedByteArrayParam(Delta.BufferSize);
        public readonly StringParam Shared = new StringParam(isOptional: true);
        
        public GameWorldEvent() : base(Event.OperationCode.EcsWorld)
        {
            AddParam(ReferenceFrame);
            AddParam(Frame);
            AddParam(Data);
            AddParam(Shared);
        }
    }
}