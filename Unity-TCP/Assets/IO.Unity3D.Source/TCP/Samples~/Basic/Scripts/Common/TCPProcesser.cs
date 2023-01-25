using System;
using System.Reflection;
using Google.Protobuf;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 22:56
    //******************************************
    public class TCPProcesser
    {
        public readonly MessageParser Parser;
        public readonly Object Instance;
        public readonly MethodInfo Method;

        private object[] _Params = new object[2];

        public TCPProcesser(MessageParser parser, object instance, MethodInfo method)
        {
            Parser = parser;
            Instance = instance;
            Method = method;
        }

        public void Process(ITCPContext ctx, IMessage message)
        {
            _Params[0] = ctx;
            _Params[1] = message;
            Method.Invoke(Instance, _Params);
        }
    }
}