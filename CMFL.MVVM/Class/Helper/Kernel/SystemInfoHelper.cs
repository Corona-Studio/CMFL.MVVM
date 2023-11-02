using System;
using System.Collections.Generic;
using System.Globalization;

namespace CMFL.MVVM.Class.Helper.Kernel
{
    public static class SystemInfoHelper
    {
        static SystemInfoHelper()
        {
            SysInfo = GetSystemInfo();
        }

        public static Dictionary<string, string> SysInfo { get; private set; }

        /// <summary>
        ///     获取操作系统的基础数据
        /// </summary>
        /// <returns>返回Dictionary，包含系统的各类数据</returns>
        private static Dictionary<string, string> GetSystemInfo()
        {
            SysInfo = new Dictionary<string, string>
            {
                {"Language", CultureInfo.InstalledUICulture.Name},
                {"SystemVersionMajor", Environment.OSVersion.Version.Major.ToString()},
                {"SystemVersionBuild", Environment.OSVersion.Version.Build.ToString()},
                {"SystemVersionMinor", Environment.OSVersion.Version.Minor.ToString()},
                {"SystemVersionRevision", Environment.OSVersion.Version.Revision.ToString()},
                {"SystemVersionMajorRevision", Environment.OSVersion.Version.MajorRevision.ToString()},
                {"SystemVersionMinorRevision", Environment.OSVersion.Version.MinorRevision.ToString()},
                {"SystemVersionServicePack", Environment.OSVersion.ServicePack},
                {
                    "SysModel",
                    CultureInfo.InstalledUICulture.Name == "zh-CN" ? "系统型号" : "System Model"
                },
                {"OperatingSystemInfo", Environment.Is64BitOperatingSystem ? "64Bits" : "32Bits"},
                {"UserName", Environment.UserName}
            };
            return SysInfo;
        }
    }
}