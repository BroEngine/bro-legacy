using System;
using System.Net;

namespace Bro.Network.Udp.Engine
{
    public static class NetUtils
    {
        public static DeliveryMethod GetSendOptions(bool isReliable, bool isOrdered)
        {
            DeliveryMethod options;
            if (isReliable)
            {
                options = isOrdered ? DeliveryMethod.ReliableOrdered : DeliveryMethod.ReliableUnordered;
            }
            else
            {
                options = isOrdered ? DeliveryMethod.Sequenced : DeliveryMethod.Unreliable;
            }

            return options;
        }
        
        public static IPEndPoint MakeEndPoint(string hostStr, int port)
        {
            return new IPEndPoint(ResolveAddress(hostStr), port);
        }

        private static IPAddress ResolveAddress(string hostStr)
        {
            if (hostStr == "localhost")
            {
                return IPAddress.Loopback;
            }

            IPAddress ipAddress;
            IPAddress.TryParse(hostStr, out ipAddress);

            if (ipAddress == null)
            {
                throw new ArgumentException("Invalid address: " + hostStr);
            }

            return ipAddress;
        }

        internal static int RelativeSequenceNumber(int number, int expected)
        {
            return (number - expected + NetConstants.MaxSequence + NetConstants.HalfMaxSequence) % NetConstants.MaxSequence - NetConstants.HalfMaxSequence;
        }
    }
}
