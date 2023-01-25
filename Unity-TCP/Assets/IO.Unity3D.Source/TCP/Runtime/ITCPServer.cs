namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 21:29
    //******************************************
    public interface ITCPServer
    {
        void Listen(int port);

        void Close();
    }
}