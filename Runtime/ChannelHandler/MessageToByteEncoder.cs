namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 21:08
    //******************************************
    public abstract class MessageToByteEncoder<T> : ChannelOutBoundHandler
    {
        private IByteBuffer _OutByteBuffer = new CircularByteBuffer();

        public override void OnWrite(ITCPContext context, object msg)
        {
            if (!(msg is T))
            {
                return;
            }

            var t = (T) msg;
            Encode(context, t, _OutByteBuffer);
            if (_OutByteBuffer.ReadableBytes() > 0)
            {
                context.Write(_OutByteBuffer);
            }
        }

        protected abstract void Encode(ITCPContext context, T t, IByteBuffer outBuffer);
    }
}