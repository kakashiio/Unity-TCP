using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 17:41
    //******************************************
    public class TCPClient : ITCPClient
    {
        private TCPContext _Ctx;
        private Socket _Socket;
        private string _Host;
        private int _Port;
        private Func<IChannelHandler> _ChannelHandlerCreator;

        public TCPClient(Func<IChannelHandler> channelHandlerCreator)
        {
            _ChannelHandlerCreator = channelHandlerCreator;
        }

        public void Connect(string host, int port)
        {
            _Host = host;
            _Port = port;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Ctx = new TCPContext("Client", _Socket, _ChannelHandlerCreator());
            _Socket.Connect(host, port);
            _Ctx.OnConnected();
        }

        public async Task ConnectAsync(string host, int port)
        {
            _Host = host;
            _Port = port;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Ctx = new TCPContext("Client", _Socket, _ChannelHandlerCreator());
            await _Socket.ConnectAsync(host, port);
            _Ctx.OnConnected();
        }

        public void Write(object data)
        {
            _Ctx.Write(data);
        }

        public void Close()
        {
            _Ctx.Close();
        }

        public override string ToString()
        {
            return $"{_Host}:{_Port}";
        }
    }
}