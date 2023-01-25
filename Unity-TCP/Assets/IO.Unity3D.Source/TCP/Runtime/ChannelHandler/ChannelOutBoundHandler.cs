using System;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 16:08
    //******************************************
    public abstract class ChannelOutBoundHandler : IChannelHandler
    {
        public virtual void OnConnected(ITCPContext context)
        {
        }

        public abstract void OnWrite(ITCPContext context, object msg);

        public void OnReceive(ITCPContext context, Object msg)
        {
        }

        public virtual void OnClose(TCPContext context)
        {
        }
        public virtual void OnException(TCPContext context, Exception exception)
        {
        }
    }
}