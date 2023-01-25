using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    public class TestTCPClient : MonoBehaviour
    {
        public string Header = "2023";
        public string Host = "127.0.0.1";
        public int Port = 8123;

        private ITCPClient _Client;
        private ITCPServer _Server;
        
        async Task Start()
        {
            TCPLogger.LogLevel = TCPLogger.LogLevelType.Debug;
            var headers = Encoding.UTF8.GetBytes(Header);
            _StartServer(headers, Port);
            await _StartClient(headers, Host, Port);
        }

        private void OnGUI()
        {
            if (_Client == null)
            {
                return;
            }

            GUI.skin.button.fontSize = 36;
            
            if (GUI.Button(new Rect(10, 10, 300, 80), "[Client] Close"))
            {
                _Client.Close();
            }
            
            if (GUI.Button(new Rect(10, 100, 300, 80), "[Client] Connect"))
            {
                _Client.ConnectAsync(Host, Port);
            }
            
            if (GUI.Button(new Rect(320, 10, 300, 80), "[Server] Close"))
            {
                _Server.Close();
            }
            
            if (GUI.Button(new Rect(320, 100, 300, 80), "[Server] Start"))
            {
                _Server.Listen(Port);
            }
            
            if (GUI.Button(new Rect(10, 190, 300, 80), "Login"))
            {
                _Client.Write(new Package { OPCode = OPCodes.REQ_LOGIN, Proto = new c2g_plrLogin()
                {
                    AccountId = 1000,
                    IconId = 2000,
                    ModelId = 3000,
                    ServerId = 4000,
                    UserName = "5000"
                }});
            }
            
            if (GUI.Button(new Rect(10, 280, 300, 80), "Loginx10"))
            {
                for (int i = 0; i < 10; i++)
                {
                    _Client.Write(new Package { OPCode = OPCodes.REQ_LOGIN, Proto = new c2g_plrLogin()
                    {
                        AccountId = 1000 + i,
                        IconId = 2000 + i,
                        ModelId = 3000 + i,
                        ServerId = 4000 + i,
                        UserName = (5000 + i).ToString()
                    }});   
                }
            }
        }

        private async Task _StartClient(byte[] headers, string host, int port)
        {
            Dictionary<short, TCPProcesser> processers = new Dictionary<short, TCPProcesser>();
            TCPProcesserHelper.FindProcesser(processers, new ClientLoginProcesser());
            ITCPClient client = new TCPClient(() => new ChannelHandlers(
                    new LoggerHandler("Client"),
                    new PackageDecodeHandler("Client", headers, processers), 
                    new PackageEncodeHandler("Client", headers)
                )
            );
            TCPLogger.LogDebug("Client", "Connecting");
            await client.ConnectAsync(host, port);
            TCPLogger.LogDebug("Client", "Finish connected");
            
            _Client = client;
        }

        private void _StartServer(byte[] headers, int port)
        {
            Dictionary<short, TCPProcesser> processers = new Dictionary<short, TCPProcesser>();
            TCPProcesserHelper.FindProcesser(processers, new ServerLoginProcesser());
            
            _Server = new TCPServer(() =>
            {
                return new ChannelHandlers(
                    new LoggerHandler("Server"),
                    new PackageDecodeHandler("Server", headers, processers),
                    new PackageEncodeHandler("Server", headers)
                );
            });
            _Server.Listen(port);
        }
    }
}
