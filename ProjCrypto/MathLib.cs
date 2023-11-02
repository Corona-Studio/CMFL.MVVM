using System;

namespace ProjCrypto
{
    internal class MathLib
    {
        /// <summary>
        ///     将DateTime转换为UNIX时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static int ConvertDateTimeInt(DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int) (time - startTime).TotalSeconds;
        }
    }
}