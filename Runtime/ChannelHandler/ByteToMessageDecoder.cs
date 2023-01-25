using UnityEngine;
using Object = System.Object;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 21:08
    //******************************************
    public abstract class ByteToMessageDecoder : ChannelInBoundHandler
    {
        public override void OnReceive(ITCPContext context, Object msg)
        {
            if (msg == null || !(msg is IByteBuffer))
            {
                return;
            }

            var byteBuffer = (IByteBuffer)msg;
            if (byteBuffer.ReadableBytes() <= 0)
            {
                return;
            }

            while (true)
            {
                if (!Decode(context, byteBuffer))
                {
                    break;
                }
            }
        }
        
        protected abstract bool Decode(ITCPContext context, IByteBuffer inBuffer);
    }
}