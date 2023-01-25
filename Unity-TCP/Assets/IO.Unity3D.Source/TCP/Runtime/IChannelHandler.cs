using System;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 17:59
    //******************************************
    public interface IChannelHandler
    {
        void OnConnected(ITCPContext context);
        void OnWrite(ITCPContext context, Object msg);
        void OnReceive(ITCPContext context, Object msg);
        void OnClose(TCPContext context);
        void OnException(TCPContext context, Exception exception);
    }
}