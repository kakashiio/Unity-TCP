using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 22:56
    //******************************************
    public class PackageDecodeHandler : ByteToMessageDecoder
    {
        private const int STATUS_READING_HEADER = 0;
        private const int STATUS_READING_LENGTH_AND_OPCODE = 1;
        private const int STATUS_READING_BODY = 2;
        
        private int _Status = STATUS_READING_HEADER;

        private byte[] _Headers;
        private short _Length;
        private short _OPCode;
        private string _Name;
        private Dictionary<short, TCPProcesser> _OPCode2MessageProcessers;

        public PackageDecodeHandler(string name, byte[] headers, Dictionary<short, TCPProcesser> opCode2MessageProcessers)
        {
            _Name = name;
            _Headers = headers;
            _OPCode2MessageProcessers = opCode2MessageProcessers;
        }

        protected override bool Decode(ITCPContext context, IByteBuffer inBuffer)
        {
            TCPLogger.LogVerbose(_Name, "Decoder Start. status={0}, inBuffer={1}", _Status, inBuffer);
            if (_Status == STATUS_READING_HEADER)
            {
                if (inBuffer.ReadableBytes() < _Headers.Length)
                {
                    TCPLogger.LogVerbose(_Name, "Decoder Waiting header. status={0}, inBuffer={1}", _Status, inBuffer);
                    return false;
                }

                inBuffer.SaveReaderIndex();
                
                var header = inBuffer.ReadBytes(_Headers.Length);
                
                bool isValid = true;
                for (int i = 0; i < _Headers.Length; i++)
                {
                    if (_Headers[i] != header[i])
                    {
                        Debug.Log($"Thread#{Thread.CurrentThread.ManagedThreadId} - [{_Name}] Decoder Invalid header at #{i} expect={_Headers[i]} actual={header[i]}. readable={inBuffer.ReadableBytes()}");
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    inBuffer.LoadReaderIndex();
                    TCPLogger.LogVerbose(_Name, "Decoder Not valid header. status={0}, inBuffer={1}", _Status, inBuffer);
                    return false;
                }
                else
                {
                    _Status = STATUS_READING_LENGTH_AND_OPCODE;
                }
            }

            if (_Status == STATUS_READING_LENGTH_AND_OPCODE)
            {
                if (inBuffer.ReadableBytes() < 4)
                {
                    // 2 bytes length + 2 bytes opcode
                    TCPLogger.LogVerbose(_Name, "Decoder Waiting length and opcode. status={0}, inBuffer={1}", _Status, inBuffer);
                    return false;
                }

                _Length = inBuffer.ReadShort(false);
                _Length -= 2;
                
                _OPCode = inBuffer.ReadShort(true);
                _Status = STATUS_READING_BODY;
            }

            if (_Status == STATUS_READING_BODY)
            {
                if (inBuffer.ReadableBytes() < _Length)
                {
                    TCPLogger.LogVerbose(_Name, "Decoder Waiting body. status={0}, inBuffer={1}", _Status, inBuffer);
                    return false;
                }

                var bytes = inBuffer.ReadBytes(_Length);
                
                if (_OPCode2MessageProcessers.TryGetValue(_OPCode, out TCPProcesser processer))
                {
                    var message = processer.Parser.ParseFrom(bytes);
                    processer.Process(context, message);
                }
                else
                {
                    Debug.LogError($"Unknow processer for opcode={_OPCode}");
                }
                
                _Status = STATUS_READING_HEADER;
                TCPLogger.LogVerbose(_Name, "Decoder Read body. status={0}, inBuffer={1}", _Status, inBuffer);
                return true;
            }

            return false;
        }
    }
}