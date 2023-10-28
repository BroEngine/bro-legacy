namespace Bro.Sketch.Server
{
    #pragma warning disable 660,661
    public class BattleServerIdentifier
    {
        public readonly int Identifier;
        public readonly string Ip;
        public readonly int PortUdp;
        public readonly int PortTcp;

        public BattleServerIdentifier(int id, string ip, int udp, int tcp)
        {
            Identifier = id;
            Ip = ip;
            PortUdp = udp;
            PortTcp = tcp;
        }

        public override string ToString()
        {
            return Ip + ":(" + PortUdp + "|" + PortTcp + ")";
        }

        public static bool operator == (BattleServerIdentifier p1, BattleServerIdentifier p2)
        {
            var p1IsNull = ReferenceEquals(p1, null);
            var p2IsNull = ReferenceEquals(p2, null);
            
            if (p1IsNull && p2IsNull)
            {
                return true;
            }

            if (!p1IsNull && !p2IsNull)
            {
                return p1.Identifier == p2.Identifier;
            }
            
            return false;
        }

        public static bool operator != (BattleServerIdentifier p1, BattleServerIdentifier p2)
        {
            return !(p1 == p2);
        }
    }
}