using System.Threading.Tasks;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 17:39
    //******************************************
    public interface ITCPClient
    {
        void Connect(string host, int port);
        Task ConnectAsync(string host, int port);
        void Write(object data);

        void Close();
    }
}