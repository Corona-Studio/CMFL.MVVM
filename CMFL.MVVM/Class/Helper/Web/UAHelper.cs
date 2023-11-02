using CMFL.MVVM.Class.Helper.Launcher.Settings;

namespace CMFL.MVVM.Class.Helper.Web
{
    public static class UAHelper
    {
        /// <summary>
        ///     随机返回一个UA标识字符串
        /// </summary>
        /// <returns>返回一个随机的UA字符串</returns>
        public static string GenerateUa()
        {
            /*
            string[] userAgent =
            {
                "CMFL/1.71.11 HOLA",
                "CMFL/1.71.11 WOW",
                "CMFL/1.71.11 YEAH",
                "CMFL/1.71.11 HI",
                "CMFL/1.71.11 HELLO",
            };

            var itemNumber = Enumerable.Range(0, userAgent.Length).RandomSample();
            */
            return
                $"CMFL/{SettingsHelper.LauncherMajor}.{SettingsHelper.LauncherMinor}.{SettingsHelper.LauncherMirror}";
        }
    }
}