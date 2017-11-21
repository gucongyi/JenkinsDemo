using UnityEngine;
using System;
using System.Collections.Generic;

namespace GPCommon
{
    public class Watchdog
    {
        public class LogMessage
        {
            public enum Type
            {
                Info,
                Warning,
                Error,
                Exception
            }

            public Type MessageType;
            public string CreateTime;
            public string Message;
            public string StackTrace;
            public string Tag;

            public override string ToString()
            {
                var color = "white";

                switch (MessageType)
                {
                    case Type.Info:
                        color = "white";
                        break;
                    case Type.Warning:
                        color = "yellow";
                        break;
                    case Type.Error:
                        color = "#ff6699ff";
                        break;
                    case Type.Exception:
                        color = "#ff6699ff";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var tagStr = Tag != "" ? "<" + Tag + ">" : "";

                var stackTraceStr = MessageType == Type.Exception ? "\n<color=cyan>" + StackTrace + "</color>" : "";
                var messageStr = Message + stackTraceStr;

                return string.Format("{0}<color={1}>[{2}]{3} {4}</color>\r\n",
                    "", color,
                    MessageType,
                    tagStr,
                    messageStr);
            }

            public bool CheckTagFilter(List<string> tagFilter)
            {
                if (tagFilter == null) return true;

                var index = tagFilter.FindIndex((oneTag) => oneTag == Tag);
                return index != -1;
            }

            public bool CheckTypeFilter(List<Type> typeFilter)
            {
                if (typeFilter == null) return true;

                var index = typeFilter.FindIndex((oneType) => oneType == MessageType);
                return index != -1;
            }
        }

        private const int MaxLogCount = 1000;
        private const int MaxCharacterLength = 15000;

        public static List<LogMessage> Logs;
        public static List<string> Tags;

        private static bool _isInited;

        internal static void Init()
        {
            if (_isInited)
                return;

            Logs = new List<LogMessage>();
            Tags = new List<string>();

            // Register callback
            Application.logMessageReceived += DebugLogCallback;

            _isInited = true;

            // Log success info
            Log("Watchdog", "**Initialized**");
        }

        private static bool _logHandlerSwith = true;

        public static void DebugLogCallback(string logString, string stackTrace, LogType type)
        {
            if (!_logHandlerSwith) return;

#if DEBUG_CONSOLE
            switch (type)
            {
                case LogType.Log:
                    AddToLogs("Debug.Log", logString);
                    break;
                case LogType.Warning:
                    AddToLogs("Debug.Warning", logString, null, LogMessage.Type.Warning);
                    break;
                case LogType.Error:
                    AddToLogs("Debug.Error", logString, null, LogMessage.Type.Error);
                    break;
                case LogType.Assert:
                    AddToLogs("Debug.Assert", logString);
                    break;
                case LogType.Exception:
                    AddToLogs("Debug.Exception", logString, stackTrace, LogMessage.Type.Exception);
                    break;
            }
#endif
        }

        private const string Default = "default";

        public static void Log(string message)
        {
            if (!_isInited) return;

            var log = AddToLogs(Default, message);

#if UNITY_EDITOR
            _logHandlerSwith = false;
            Debug.Log(string.Format("{0} {1}", log, log.CreateTime));
            _logHandlerSwith = true;
#endif
        }

        public static void Log(string tag, string message)
        {
            if (!_isInited) return;

            var log = AddToLogs(tag, message);

#if UNITY_EDITOR
            _logHandlerSwith = false;
            Debug.Log(string.Format("{0} {1}", log, log.CreateTime));
            _logHandlerSwith = true;
#endif
        }

        public static void LogWarning(string tag, string message)
        {
            if (!_isInited) return;

            var log = AddToLogs(tag, message, null, LogMessage.Type.Warning);

#if UNITY_EDITOR
            _logHandlerSwith = false;
            Debug.LogWarning(string.Format("{0} {1}", log, log.CreateTime));
            _logHandlerSwith = true;
#endif
        }

        public static void LogError(string tag, string message)
        {
            if (!_isInited) return;

            var log = AddToLogs(tag, message, null, LogMessage.Type.Error);

#if UNITY_EDITOR
            _logHandlerSwith = false;
            Debug.LogError(string.Format("{0} {1}", log, log.CreateTime));
            _logHandlerSwith = true;
#endif
        }

        public static void LogCaughtException(string tag, Exception e)
        {
            if (!_isInited) return;

            var log = AddToLogs(tag, e.Message, e.StackTrace, LogMessage.Type.Exception);
#if UNITY_EDITOR
            _logHandlerSwith = false;
            Debug.LogError(string.Format("{0} {1}", log, log.CreateTime));
            _logHandlerSwith = true;
#endif
        }

        private static LogMessage AddToLogs(string tag, string message,
            string stackTrace = null,
            LogMessage.Type type = LogMessage.Type.Info)
        {
#if DEBUG_CONSOLE

            var log = new LogMessage()
            {
                Message = message,
                MessageType = type,
                StackTrace = stackTrace,
                Tag = tag,
                CreateTime = DateTime.Now.ToString("yyyy.MM.dd  HH:mm:ss  fff"),
            };

            // Add logs
            Logs.Add(log);

            // Check capacity
            while (Logs.Count > MaxLogCount)
            {
                Logs.RemoveAt(0);
            }

            return log;
#else
            return null;
#endif
        }

        public static string ToString(List<string> tagFilter = null, List<LogMessage.Type> typeFilter = null)
        {
            // Prepare pending list
            List<LogMessage> pendingList = Logs.FindAll((log) =>
            {
                // Check all filters
                if (log.CheckTagFilter(tagFilter) && log.CheckTypeFilter(typeFilter))
                    return true;

                return false;
            });

            // Prepare pending string
            string pendingStr = "";
            for (int i = pendingList.Count - 1; i >= 0; i--)
            {
                var t = pendingList[i] + pendingStr;

                if (t.Length >= MaxCharacterLength) break;

                pendingStr = t;
            }

            return pendingStr;
        }

        public static void Clear()
        {
            Logs.Clear();
        }
    }
}