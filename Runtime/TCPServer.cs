using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 21:30
    //******************************************
    public class TCPServer : ITCPServer
    {
        private volatile TcpListener _TCPListener;
        private volatile bool _Close;
        private Func<IChannelHandler> _ChannelHandlerCreator;
        private ConcurrentQueue<Socket> _Sockets;

        public TCPServer(Func<IChannelHandler> channelHandlerCreator)
        {
            _ChannelHandlerCreator = channelHandlerCreator;
        }

        public void Listen(int port)
        {
            if (_TCPListener != null)
            {
                TCPLogger.LogError("Server", "Server is listening, invoke `Close` first if you want to listen again.");
                return;
            }

            _TCPListener = new TcpListener(port);
            _TCPListener.Start();
            _Sockets = new ConcurrentQueue<Socket>();
            new Task(_Accept).Start();
            _Close = false;
        }
        
        private void _Accept()
        {
            while (!_Close)
            {
                var socket = _TCPListener.AcceptSocket();
                _Sockets.Enqueue(socket);
                new Task(() => _ProcessClient(socket)).Start();
            }
        }

        private void _ProcessClient(Socket socket)
        {
            var ctx = new TCPContext("Server", socket, _ChannelHandlerCreator());
            ctx.OnConnected();
        }

        public void Close()
        {
            _Close = true;
            if (_TCPListener != null)
            {
                _TCPListener.Stop();
                var sockets = _Sockets;
                while (sockets.TryDequeue(out Socket s))
                {
                    s.Close();
                }
                _TCPListener = null;
            }
        }
    }
}