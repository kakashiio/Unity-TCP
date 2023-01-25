using System;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 23:03
    //******************************************
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TCPProcesserAttribute : Attribute
    {
        public short OPCode;

        public TCPProcesserAttribute(short opCode)
        {
            OPCode = opCode;
        }
    }
}