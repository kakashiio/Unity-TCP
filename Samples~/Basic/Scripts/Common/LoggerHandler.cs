using System;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-24 18:11
    //******************************************
    public class LoggerHandler : IChannelHandler
    {
        private string _Name;

        public LoggerHandler(string name)
        {
            _Name = name;
        }

        public void OnConnected(ITCPContext context)
        {
            TCPLogger.LogDebug(_Name, "Logger OnConnected");
        }

        public void OnWrite(ITCPContext context, object msg)
        {
            TCPLogger.LogVerbose(_Name, "Logger OnWrite");
        }

        public void OnReceive(ITCPContext context, object msg)
        {
            TCPLogger.LogVerbose(_Name, "Logger OnReceive");
        }

        public void OnClose(TCPContext context)
        {
            TCPLogger.LogDebug(_Name, "Logger OnClose");
        }

        public void OnException(TCPContext context, Exception exception)
        {
            TCPLogger.LogException(_Name, "Logger", exception);
        }
    }
}