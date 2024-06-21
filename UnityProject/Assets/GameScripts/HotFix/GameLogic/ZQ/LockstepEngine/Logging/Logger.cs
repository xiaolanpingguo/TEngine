using System;
using System.Diagnostics;
using System.Text;


namespace Lockstep.Framework 
{
    [Flags]
    public enum LogType
    {
        Error = 1,
        Warn = 2,
        Info = 4,
    }

    public static class LTLog 
    {
        public static LogType LogLevel = LogType.Info | LogType.Warn | LogType.Error;
        public static Action<LogType, string> OnMessage = DefaultServerLogHandler;


        private static StringBuilder _logBuffer = new StringBuilder();

        public static void Error(string message, params object[] args)
        {
            LogMessage(LogType.Error, message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            LogMessage(LogType.Warn, message, args);
        }

        public static void Info(string message, params object[] args)
        {
            LogMessage(LogType.Info, message, args);
        }

        public static void Assert(bool val, string message)
        {
            if (!val)
            {
                LogMessage(LogType.Error, "AssertFailed!!! " + message);
            }
        }

        private static void LogMessage(LogType type, string format, params object[] args)
        {
            if (OnMessage != null && (LogLevel & type) != 0) 
            {
                var message = (args != null && args.Length > 0) ? string.Format(format, args) : format;
                OnMessage.Invoke(type, message);
            }
        }
        
        public static void DefaultServerLogHandler(LogType type, string log)
        {
            if ((LogType.Error & type) != 0) 
            {
                StackTrace st = new StackTrace(true);
                StackFrame[] sf = st.GetFrames();
                for (int i = 4; i < sf.Length; ++i) 
                {
                    var frame = sf[i];
                    _logBuffer.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" + frame.GetMethod().Name +" Line=" + frame.GetFileLineNumber());
                }
            }

#if UNITY_EDITOR
            switch(type)
            {
                case LogType.Info:
                    {
                        UnityEngine.Debug.Log(log);
                        break;
                    }
                case LogType.Warn:
                    {
                        UnityEngine.Debug.LogWarning(log);
                        break;
                    }
                case LogType.Error:
                    {
                        UnityEngine.Debug.LogError(log);
                        UnityEngine.Debug.LogError(_logBuffer.ToString());
                        break;
                    }
            }
#else
            Console.WriteLine(log);
            Console.WriteLine(_logBuffer.ToString());
#endif
            _logBuffer.Clear();
        }
    }
}