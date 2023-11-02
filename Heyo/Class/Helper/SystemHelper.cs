using System;

namespace Heyo.Class.Helper
{
    public class SystemHelper
    {
        static SystemHelper()
        {
            var v = Environment.OSVersion.Version;
            IsWindow10 = v.Major == 6 && v.Build == 9200; //判断是否为win10
        }

        public static bool IsWindow10 { get; }
    }
}