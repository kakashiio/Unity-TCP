using System.Threading;
using UnityEngine;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 22:55
    //******************************************
    public class ClientLoginProcesser
    {
        [TCPProcesser(OPCodes.RESP_LOGIN)]
        public void OnLogin(ITCPContext ctx, g2c_plrLogin login)
        {
            TCPLogger.LogDebug("Client", "Login AccountId={0}, ServerId={1}", login.AccountId, login.ServerId);
        }
    }
}