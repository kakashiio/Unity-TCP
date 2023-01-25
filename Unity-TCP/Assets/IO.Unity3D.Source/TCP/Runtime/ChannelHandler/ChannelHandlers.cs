using System;
using System.Collections.Generic;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 18:02
    //******************************************
    public class ChannelHandlers : IChannelHandler
    {
        private List<IChannelHandler> _ChannelHandlers;

        public ChannelHandlers(IEnumerable<IChannelHandler> channelHandlers)
        {
            _ChannelHandlers = channelHandlers == null ? new List<IChannelHandler>(0) : new List<IChannelHandler>(channelHandlers);
        }
        
        public ChannelHandlers(params IChannelHandler[] channelHandlers)
        {
            _ChannelHandlers = channelHandlers == null ? new List<IChannelHandler>(0) : new List<IChannelHandler>(channelHandlers);
        }

        public void OnConnected(ITCPContext context)
        {
            for (int i = 0; i < _ChannelHandlers.Count; i++)
            {
                _ChannelHandlers[i].OnConnected(context);
            }
        }

        public void OnWrite(ITCPContext context, object msg)
        {
            for (int i = 0; i < _ChannelHandlers.Count; i++)
            {
                _ChannelHandlers[i].OnWrite(context, msg);
            }
        }

        public void OnReceive(ITCPContext context, Object msg)
        {
            for (int i = 0; i < _ChannelHandlers.Count; i++)
            {
                _ChannelHandlers[i].OnReceive(context, msg);
            }
        }

        public void OnClose(TCPContext context)
        {
            for (int i = 0; i < _ChannelHandlers.Count; i++)
            {
                _ChannelHandlers[i].OnClose(context);
            }
        }

        public void OnException(TCPContext context, Exception exception)
        {
            for (int i = 0; i < _ChannelHandlers.Count; i++)
            {
                _ChannelHandlers[i].OnException(context, exception);
            }
        }
    }
}