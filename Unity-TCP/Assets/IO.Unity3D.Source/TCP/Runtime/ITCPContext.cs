namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 18:37
    //******************************************
    public interface ITCPContext
    {
        public void Write(object data, bool flush = true);
        
        public void Write(byte data, bool flush = true);
        
        public void Write(IByteBuffer data, bool flush = true);
        
        public void Write(byte[] data, int offset, int count, bool flush = true);

        public void Close();
    }
}