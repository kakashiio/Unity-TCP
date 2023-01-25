using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf;
using UnityEngine;

namespace IO.Unity3D.Source.TCP.Samples.Basic
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-23 23:05
    //******************************************
    public class TCPProcesserHelper
    {
        public static void FindProcesser(Dictionary<short, TCPProcesser> result, object obj)
        {
            var type = obj.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                var tcpProcesserAttribute = method.GetCustomAttribute<TCPProcesserAttribute>();
                if (tcpProcesserAttribute == null)
                {
                    continue;
                }

                short opCode = tcpProcesserAttribute.OPCode;

                var parameters = method.GetParameters();
                if (parameters == null || parameters.Length != 2)
                {
                    Debug.Log($"Found {nameof(TCPProcesser)} method, but doesn't have {nameof(ITCPContext)} adn {nameof(IMessage)} arguments. {type}#{method.Name}");
                    continue;
                }

                var ctxType = parameters[0].ParameterType;
                var msgType = parameters[1].ParameterType;

                if (!ctxType.IsAssignableFrom(typeof(ITCPContext)))
                {
                    Debug.Log($"Found {nameof(TCPProcesser)} method, but argument 0 is not type of {nameof(ITCPContext)}. {type}#{method.Name}");
                    continue;
                }
                if (!typeof(IMessage).IsAssignableFrom(msgType))
                {
                    Debug.Log($"Found {nameof(TCPProcesser)} method, but argument 1 is not type of {nameof(IMessage)}. {type}#{method.Name}");
                    continue;
                }

                if (result.TryGetValue(opCode, out TCPProcesser processer))
                {
                    Debug.Log($"Duplicate processer for opcode={opCode} old={processer.Instance.GetType()}#{processer.Method.Name}. {type}#{method.Name}");
                    continue;
                }

                var parserProperty = msgType.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public);
                var parser = parserProperty.GetValue(msgType) as MessageParser;

                if (parser == null)
                {
                    Debug.Log($"Find no parser. {type}#{method.Name}");
                    continue;
                }

                result.Add(opCode, new TCPProcesser(parser, obj, method));
            }
        }
        
        public static void FindProcesser(Dictionary<short, TCPProcesser> result, IEnumerable<object> objects)
        {
            if (objects == null)
            {
                return;
            }

            foreach (var type in objects)
            {
                FindProcesser(result, type);
            }
        }
        
        public static void FindProcesser(Dictionary<short, TCPProcesser> result, params object[] objects)
        {
            if (objects == null)
            {
                return;
            }

            foreach (var type in objects)
            {
                FindProcesser(result, type);
            }
        }
    }
}