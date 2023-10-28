// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------


using System;
using System.Net;
using System.Net.Sockets;
using Bro.Threading;

namespace Bro.Network.Udp.Engine
{
    internal interface INetSocketListener
    {
        void OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint);
    }

    internal sealed class NetSocket
    {
        private const int ReceivePollingTime = 500000; // 0.5 second
        private Socket _udpSocket;
        private BroThread _thread;
        private readonly INetSocketListener _listener;
        private const int SioUdpConnReset = -1744830452; // SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12
        
        public volatile bool IsRunning;

        public NetSocket(INetSocketListener listener)
        {
            _listener = listener;
        }

        private bool IsActive()
        {
            return IsRunning;
        }

        private void ReceiveLogic(object state)
        {
            var socket = (Socket)state;
            EndPoint bufferEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var receiveBuffer = new byte[NetConstants.MaxPacketSize];

            while (IsActive())
            {
                int result;
                
                try
                {
                    if (socket.Available == 0 && !socket.Poll(ReceivePollingTime, SelectMode.SelectRead))
                    {
                        continue;
                    }
                    
                    var point = PerformanceMeter.Register( PerformancePointType.UdpKernelRead );
                    result = socket.ReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref bufferEndPoint);
                    point?.Done();
                }
                catch (SocketException ex)
                {
                    _listener.OnMessageReceived(null, 0, ex.SocketErrorCode, (IPEndPoint)bufferEndPoint);
                    
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                
                _listener.OnMessageReceived(receiveBuffer, result, 0, (IPEndPoint)bufferEndPoint);
            }
        }

        public bool Bind(IPAddress addressIPv4, IPAddress addressIPv6, int port)
        {
            if (IsActive())
            {
                return false;
            }

            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (!BindSocket(_udpSocket, new IPEndPoint(addressIPv4, port), false))
            {
                return false;
            }

            IsRunning = true;

            _thread = new BroThread(ReceiveLogic);
            _thread.IsBackground = true;
            _thread.Start(_udpSocket);

            return true;
        }

        private bool BindSocket(Socket socket, IPEndPoint ep, bool reuseAddress)
        {
            socket.ReceiveTimeout = 500;
            socket.SendTimeout = 500;
            socket.ReceiveBufferSize = NetConstants.SocketBufferSize;
            socket.SendBufferSize = NetConstants.SocketBufferSize;
            
            try
            {
                socket.IOControl(SioUdpConnReset, new byte[] { 0 }, null);
            }
            catch { /* ignored */ }

            try
            {
                socket.ExclusiveAddressUse = !reuseAddress;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
            }
            catch { /* ignored */ }
            
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                socket.Ttl = NetConstants.SocketTtl;

                try
                {
                    socket.DontFragment = true;
                }
                catch (SocketException e)
                {
                    Bro.Log.Error(e);
                }

                try
                {
                    socket.EnableBroadcast = true;
                }
                catch (SocketException e)
                {
                    Bro.Log.Error(e);
                }
            }

            try
            {
                socket.Bind(ep);
            }
            catch (SocketException bindException)
            {
                Bro.Log.Error(bindException);
                return false;
            }
            return true;
        }

        public int SendTo(byte[] data, int offset, int size, IPEndPoint remoteEndPoint, ref SocketError errorCode)
        {
            if (!IsActive())
            {
                return 0;
            }
            
            try
            {
                var point = PerformanceMeter.Register( PerformancePointType.UdpKernelWrite );
                
                var result = _udpSocket.SendTo(data, offset, size, SocketFlags.None, remoteEndPoint);
                SystemMonitoring.NetworkUdpSentBytes?.Observe(size);
                
                point?.Done();
                return result;
            }
            catch (SocketException ex)
            {
                errorCode = ex.SocketErrorCode;
                return -1;
            }
            catch (Exception ex)
            {
                Bro.Log.Error(ex);
                return -1;
            }
        }

        public void Close(bool suspend)
        {
            if (!suspend)
            {
                IsRunning = false;
            }

            if (_udpSocket != null)
            {
                _udpSocket.Close();
                _udpSocket = null;
            }

            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
        }
    }
}
