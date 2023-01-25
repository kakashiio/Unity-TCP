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
    public class ServerLoginProcesser
    {
        [TCPProcesser(OPCodes.REQ_LOGIN)]
        public void OnLogin(ITCPContext ctx, c2g_plrLogin login)
        {
            TCPLogger.LogDebug("Server", "Login AccountId={0}, ServerId={1}, IconID={2}, ModelId={3}, UserName={4}", login.AccountId, login.ServerId, login.IconId, login.ModelId, login.UserName);
            
            ctx.Write(new Package { OPCode = OPCodes.RESP_LOGIN, Proto = new g2c_plrLogin()
            {
                AccountId = login.AccountId,
                ServerId = login.ServerId,
            }});
        }
    }
}