using System;

namespace CMFL.MVVM.Class.Helper.Other
{
    public static class TimeHelper
    {
        /// <summary>
        ///     获取时间戳 10位
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampTen()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        ///     获取时间戳  13位
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds * 1000);
        }

        /// <summary>
        ///     将时间戳转换为日期类型，并格式化
        /// </summary>
        /// <param name="longDateTime"></param>
        /// <returns></returns>
        public static string LongDateTimeToDateTimeString(string longDateTime, bool isMilliseconds)
        {
            long unixDate;
            DateTime start;
            DateTime date;

            unixDate = long.Parse(longDateTime);
            start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            date = isMilliseconds
                ? start.AddMilliseconds(unixDate).ToLocalTime()
                : start.AddSeconds(unixDate).ToLocalTime();

            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static long TimeDiff(this DateTime dt1, DateTime dt2)
        {
            var ts1 = new TimeSpan(dt1.Ticks);
            var ts2 = new TimeSpan(dt2.Ticks);
            var ts3 = ts2.Subtract(ts1); //ts2-ts1
            return (long) ts3.TotalSeconds; //得到相差秒数
        }
    }
}