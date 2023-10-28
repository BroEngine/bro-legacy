namespace Bro.Sketch.Network
{
    public class KickOutEvent : Bro.Network.NetworkEvent<KickOutEvent>
    {
        public KickOutEvent() : base(Event.OperationCode.KickOut)
        {
        }
    }
}