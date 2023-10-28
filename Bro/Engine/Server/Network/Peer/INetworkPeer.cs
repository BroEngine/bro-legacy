namespace Bro.Server.Network
{
    public interface INetworkPeer
    {
        int Id { get; }

        void Send(byte[] data, bool isReliable, bool isOrdered);
        void Disconnect(int code);
        void OnConnect();
        void OnReceive(byte[] data);

        System.Action<int> OnDisconnectHandler { set; }
        System.Action OnConnectHandler { set; }
        System.Action<byte[]> OnReceiveBinaryHandler { set; }
    }
}