using System.Threading;
using Google.Protobuf;
using UnityEngine;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 22:59
    //******************************************
    public class PackageEncodeHandler : MessageToByteEncoder<Package>
    {
        private byte[] _Headers;
        private string _Name;
        public PackageEncodeHandler(string name, byte[] headers)
        {
            _Headers = headers;
            _Name = name;
        }

        protected override void Encode(ITCPContext context, Package package, IByteBuffer outBuffer)
        {
            outBuffer.WriteBytes(_Headers);

            var proto = package.Proto;
            byte[] protoDatas = null;
            int protoLength = 0;
            if (proto != null)
            {
                protoDatas = proto.ToByteArray();
                protoLength = protoDatas.Length;
            }

            int length = protoLength + 2;
            
            outBuffer.WriteUShort((ushort)length, false);
            outBuffer.WriteUShort((ushort)package.OPCode, true);

            if (protoDatas != null)
            {
                outBuffer.WriteBytes(protoDatas);
            }
            
            TCPLogger.LogVerbose(_Name, "Encoder Encode package header={0} opcode={1}, length={2}, bytes={3}", _Headers.Length, package.OPCode, length, outBuffer.ReadableBytes());
        }
    }
}