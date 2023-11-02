using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Heyo.Class.Helper
{
    public class PermissionHelper
    {
        /// <summary>
        ///     检查程序是否在管理员身份下运行
        /// </summary>
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            var current = WindowsIdentity.GetCurrent();
            var windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        ///     以管理员身份重启程序
        /// </summary>
        public static void RestartByAdministrator()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Application.ExecutablePath;
            psi.Verb = "runas";
            try
            {
                Process.Start(psi);
                System.Windows.Application.Current.Shutdown();
            }
            catch
            {
            }
        }
    }
}