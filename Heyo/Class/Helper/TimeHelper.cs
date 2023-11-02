using System;

namespace Heyo.Class.Helper
{
    public static class TimeHelper
    {
        /// <summary>
        ///     将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static double ToUnix(this DateTime time)
        {
            double intResult = 0;
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }
    }
}