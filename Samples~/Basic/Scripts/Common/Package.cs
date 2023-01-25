using Google.Protobuf;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 23:02
    //******************************************
    public class Package
    {
        public int OPCode;
        public IMessage Proto;
    }
}