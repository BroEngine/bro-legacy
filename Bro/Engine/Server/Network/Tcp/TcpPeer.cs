using System;
using Bro.Network;

namespace Bro.Server.Network.Tcp
{
    public class TcpPeer : INetworkPeer
    {
        public TcpPeer(ITransportPeer transportPeer)
        {
        }

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public void Send(byte[] data, bool isReliable, bool isOrdered)
        {
            throw new NotImplementedException();
        }

        public void OnConnect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect( int code )
        {
            throw new NotImplementedException();
        }

        public void OnReceive(byte[] data)
        {
            throw new NotImplementedException();
        }
        
        public Action<int> OnDisconnectHandler { get; set; }
        public Action OnConnectHandler { get; set; }
        public Action<byte[]> OnReceiveBinaryHandler { get; set; }

    }
}