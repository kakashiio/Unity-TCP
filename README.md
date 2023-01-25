# TCP Library in Source Framework for Unity3D

Basic TCP Client & Server Library

# How to use

## Import package

You can add package from git url through the Package Manager.

|Package|Description|
|--|--|
| https://github.com/kakashiio/Unity-TCP.git#1.0.0 | Basic TCP Client & Server Library |



## Client

You just need some step to start a client:

1. Define `Decoder`
2. Define `Encoder`
3. Start a client

> Decoder:`Decoder` is used to decode bytes received from server to message which the client want to process.

> Encoder:`Encoder` is used to encode bytes which are sent from client to server.

### Define Decoder

```csharp

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

```

### Define Encoder

```csharp

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

```

### Start a client

```csharp
    Dictionary<short, TCPProcesser> processers = new Dictionary<short, TCPProcesser>();
    TCPProcesserHelper.FindProcesser(processers, new ClientLoginProcesser());
    ITCPClient client = new TCPClient(() => new ChannelHandlers(
            new LoggerHandler("Client"),
            new PackageDecodeHandler("Client", headers, processers), 
            new PackageEncodeHandler("Client", headers)
        )
    );
    client.ConnectAsync(host, port);
```

### Complete and runnable code

**Import Samples From Package Manager and run `SampleScene.unity` scene**

## Server

You just need some step to start a client:

1. Define `Decoder`
2. Define `Encoder`
3. Start a client

> Decoder:`Decoder` is used to decode bytes received from server to message which the client want to process.

> Encoder:`Encoder` is used to encode bytes which are sent from client to server.

### Define Decoder & Encoder

Reference the `Define Decoder` & `Define Encoder` in `Client`.

### Start a server

```csharp
    Dictionary<short, TCPProcesser> processers = new Dictionary<short, TCPProcesser>();
    TCPProcesserHelper.FindProcesser(processers, new ServerLoginProcesser());
    
    var server = new TCPServer(() =>
    {
        return new ChannelHandlers(
            new LoggerHandler("Server"),
            new PackageDecodeHandler("Server", headers, processers),
            new PackageEncodeHandler("Server", headers)
        );
    });
    server.Listen(port);
```

### Complete and runnable code

**Import Samples From Package Manager and run `SampleScene.unity` scene**


## Samples

Package's samples use the google.protobuffer 3 as the data serialization scheme.