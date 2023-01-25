using System;
using System.Threading;
using UnityEngine;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-24 01:56
    //******************************************
    public class TCPLogger
    {
        public static LogLevelType LogLevel = LogLevelType.Debug;

        public enum LogLevelType
        {
            Verbose,
            Debug,
            Info,
            Warning,
            Error
        }

        public static void LogInfo(string name, string tpl, params object[] logParams)
        {
            _Log(Debug.Log, LogLevelType.Info, name, tpl, logParams);
        }
        
        public static void LogError(string name, string tpl, params object[] logParams)
        {
            _Log(Debug.Log, LogLevelType.Error, name, tpl, logParams);
        }
        
        public static void LogWarning(string name, string tpl, params object[] logParams)
        {
            _Log(Debug.Log, LogLevelType.Warning, name, tpl, logParams);
        }

        public static void LogDebug(string name, string tpl, params object[] logParams)
        {
            _Log(Debug.Log, LogLevelType.Debug, name, tpl, logParams);
        }
        
        public static void LogVerbose(string name, string tpl, params object[] logParams)
        {
            _Log(Debug.Log, LogLevelType.Verbose, name, tpl, logParams);
        }

        public static void LogException(string name, string msg, Exception exception)
        {
            LogError(name, "Exception occured while processing {0}", msg);
            Debug.LogException(exception);
        }

        private static void _Log(Action<string> logger, LogLevelType logLevel, string name, string tpl, params object[] logParams)
        {
            if (LogLevel > logLevel)
            {
                return;
            }

            if (logParams == null || logParams.Length == 0)
            {
                logger($"Thread#{Thread.CurrentThread.ManagedThreadId:X8} [{name}] - {tpl}");    
            }
            else
            {
                logger($"Thread#{Thread.CurrentThread.ManagedThreadId:X8} [{name}] - {string.Format(tpl, logParams)}");
            }
        }
    }
}