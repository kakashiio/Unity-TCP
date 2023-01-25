using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 18:33
    //******************************************
    public class TCPContext : ITCPContext
    {
        private byte[] _OneByteBuffer = new byte[1];
        
        private Socket _Socket;
        private IChannelHandler _ChannelHandler;
        private NetworkStream _Stream;
        
        private byte[] _ReceiveTmpBuffer = new byte[4];
        private IByteBuffer _ReceiveBuffer = new CircularByteBuffer();
        private string _Name;
        
        public TCPContext(string name, Socket socket, IChannelHandler channelHandler)
        {
            _Name = name;
            _Socket = socket;
            _ChannelHandler = channelHandler;
        }

        public void OnConnected()
        {
            _Stream = new NetworkStream(_Socket, true);

            TCPLogger.LogVerbose(_Name, "Context BeginReceive");
            _Socket.BeginReceive(_ReceiveTmpBuffer, 0, _ReceiveTmpBuffer.Length, SocketFlags.None, _OnReceived, this);
            _ChannelHandler.OnConnected(this);
        }

        private void _OnReceived(IAsyncResult ar)
        {
            if (_Socket == null)
            {
                return;
            }

            int read = 0;
            try
            {
                read = _Socket.EndReceive(ar);
                if (read == 0)
                {
                    Close();
                    return;
                }
            }
            catch (Exception e)
            {
                _OnException(e);
                return;
            }

            if (read > 0) {
                TCPLogger.LogVerbose(_Name, "Context Recv {0} bytes, write to temp buffer", read);
                _ReceiveBuffer.WriteBytes(_ReceiveTmpBuffer, 0, read);
                TCPLogger.LogVerbose(_Name, "Context Recv {0} bytes, invoking receive handler", read);
                _ChannelHandler.OnReceive(this, _ReceiveBuffer);
                TCPLogger.LogVerbose(_Name, "Context Recv {0} bytes, finish invoking receive handler", read);
            }
            else
            {
                TCPLogger.LogWarning(_Name, "Context Recv {0} bytes", read);
            }

            TCPLogger.LogVerbose(_Name, "Context BeginReceive");
            _Socket.BeginReceive(_ReceiveTmpBuffer, 0, _ReceiveTmpBuffer.Length, SocketFlags.None, _OnReceived, this);
        }

        public void Write(object data, bool flush = true)
        {
            if (_Socket == null)
            {
                return;
            }
            
            if (data == null)
            {
                return;
            }

            if (data is IByteBuffer)
            {
                Write((IByteBuffer) data, flush);
                return;
            }
            
            if (data is Byte)
            {
                Write((byte) data, flush);
                return;
            }
            
            if (data is byte[])
            {
                var byteArray = (byte[]) data;
                Write(byteArray, 0, byteArray.Length, flush);
                return;
            }
            
            _ChannelHandler.OnWrite(this, data);
        }

        public void Write(byte data, bool flush = true)
        {
            if (_Socket == null)
            {
                return;
            }
            
            _OneByteBuffer[0] = data;
            TCPLogger.LogVerbose(_Name, "Context Sent 1 bytes, flush={0}", flush);
            _Stream.Write(_OneByteBuffer, 0, 1);
            if (flush)
            {
                _Stream.Flush();
            }
        }

        public void Write(IByteBuffer data, bool flush = true)
        {
            if (_Socket == null)
            {
                return;
            }
            
            if (data == null)
            {
                return;
            }
            while (true)
            {
                (byte[] bytes, int offset, int count) = data.TryRead();
                if (count <= 0)
                {
                    break;
                }
                
                TCPLogger.LogVerbose(_Name, "Context Sent {0} bytes, flush={1}", count, flush);
                _Stream.Write(bytes, offset, count);
            }
            
            if (flush)
            {
                _Stream.Flush();
            }
        }

        public void Write(byte[] data, int offset, int count, bool flush = true)
        {
            if (_Socket == null)
            {
                return;
            }
            
            if (data == null)
            {
                return;
            }
            
            TCPLogger.LogVerbose(_Name, "Context Sent {0} bytes, flush={1}", count, flush);
            _Stream.Write(data, offset, count);
            if (flush)
            {
                _Stream.Flush();
            }
        }

        public void Close()
        {
            if (_Socket != null)
            {
                _OnClose();
                _Socket.Close();
                _Socket = null;
            }
        }

        private void _OnClose()
        {
            TCPLogger.LogDebug(_Name, "Context Close");
            _ChannelHandler.OnClose(this);
        }

        private void _OnException(Exception exception)
        {
            TCPLogger.LogException(_Name, "Context", exception);
            _ChannelHandler.OnException(this, exception);
        }

        public override string ToString()
        {
            return $"[{_Name}] {_Socket}";
        }
    }
}