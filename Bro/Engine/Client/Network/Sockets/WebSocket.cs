using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bro.Client.Network
{
    public class WebSocket
    {
        private readonly Uri _url;

        public WebSocket(Uri url)
        {
            _url = url;

            var protocol = _url.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            {
                throw new ArgumentException("Unsupported protocol: " + protocol);
            }
        }

        public void SendString(string str)
        {
            Send(Encoding.UTF8.GetBytes(str));
        }


#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern int SocketCreate (string url);

	[DllImport("__Internal")]
	private static extern int SocketState (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern void SocketRecv (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern int SocketRecvLength (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketClose (int socketInstance);

	[DllImport("__Internal")]
	private static extern int SocketError (int socketInstance, byte[] ptr, int length);

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend (m_NativeRef, buffer, buffer.Length);
	}

	public byte[] Recv()
	{
		int length = SocketRecvLength (m_NativeRef);
		if (length == 0)
			return null;
		byte[] buffer = new byte[length];
		SocketRecv (m_NativeRef, buffer, length);
		return buffer;
	}

	public bool Connected
	{
		get { return SocketState(m_NativeRef) != 0; }
	}

	public IEnumerator Connect()
	{
		m_NativeRef = SocketCreate (mUrl.ToString());

		while (SocketState(m_NativeRef) == 0)
			yield return 0;
	}
 
	public void Close()
	{
		SocketClose(m_NativeRef);
	}

	public string Error
	{
		get {
			const int bufsize = 1024;
			byte[] buffer = new byte[bufsize];
			int result = SocketError (m_NativeRef, buffer, bufsize);

			if (result == 0)
				return null;

			return Encoding.UTF8.GetString (buffer);				
		}
	}





#else

        Bro.Network.Tcp.Engine.Client.WebSocket _socket;
        readonly Queue<byte[]> _messagesBinary = new Queue<byte[]>();
        readonly Queue<string> _messagesJson = new Queue<string>();
        bool _isConnected = false;

        public bool Connected
        {
            get { return _isConnected; }
        }

        public IEnumerator Connect(Action callback)
        {
            _socket = new Bro.Network.Tcp.Engine.Client.WebSocket(_url.ToString());
            _socket.OnMessage += (sender, e) => { _messagesBinary.Enqueue(e.RawData); };
            _socket.OnOpen += (sender, e) => _isConnected = true;
            _socket.OnError += (sender, e) => { Error = e.ToString(); };

            _socket.OnClose += (sender, e) => { Error = "Closed"; };

            _socket.ConnectAsync();

            while (!_isConnected && Error == null)
            {
                yield return new Timing.YieldWaitForUpdate();
            }

            callback();

            yield return null;
        }


        public void Send(string data)
        {
            _socket.Send(data);
        }

        public void Send(byte[] buffer)
        {
            _socket.Send(buffer);
        }

        public byte[] RecieveBinary()
        {
            return _messagesBinary.Count == 0 ? null : _messagesBinary.Dequeue();
        }

        public string RecieveString()
        {
            return _messagesJson.Count == 0 ? null : _messagesJson.Dequeue();
        }

        public void Close()
        {
            _socket.Close();
        }

        public string Error { get; private set; }
#endif
    }
}