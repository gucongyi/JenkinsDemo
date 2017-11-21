using System;

namespace GPCommon
{
    public class DateTimeUtils
    {

        public static string StandardTimeStr
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            }
        }

        public static DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static int Timestamp
        {
            get
            {
                return (int)((DateTime.UtcNow - BaseTime).TotalSeconds);
            }
        }

        public static int GetTimeStamp(DateTime time)
        {
            return (int)((time - BaseTime).TotalSeconds - (DateTime.Now - DateTime.UtcNow).TotalSeconds);
        }

        public static long MilliTimestamp
        {
            get
            {
                return (long)((DateTime.UtcNow - BaseTime).TotalMilliseconds);
            }
        }

        public static double MicroTimestamp
        {
            get
            {
                return (DateTime.UtcNow - BaseTime).TotalMilliseconds;
            }
        }

        public static DateTime TimestampToDateTime(int timestamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(BaseTime).AddSeconds(timestamp);
        }

        public static int TimestampToDay(int timeStamp)
        {
            return timeStamp / 86400;
        }

        /// <summary>
        /// 时间戳判断是否是今天
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool CheckToday(long timestamp)
        {
            DateTime dt = BaseTime.AddMilliseconds(timestamp);
            return dt.Subtract(DateTime.Now).Days == 0;
        }
    }
}
