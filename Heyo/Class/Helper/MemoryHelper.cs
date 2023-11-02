using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Devices;

namespace Heyo.Class.Helper
{
    /// <summary>
    ///     内存控制
    /// </summary>
    [OptionText]
    [StandardModule]
    public class MemoryHelper
    {
        /// <summary>
        ///     内存单位
        /// </summary>
        public enum Unit
        {
            B,
            KB,
            MB,
            GB
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        /// <summary>
        ///     清理自身内存
        /// </summary>
        internal static void CleanSelfMemory()
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        /// <summary>
        ///     清理全局内存
        /// </summary>
        public static void CleanAllMemory()
        {
            Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                checked
                {
                    foreach (var process in processes)
                        try
                        {
                            SetProcessWorkingSetSize(process.Handle, -1, -1);
                        }
                        catch (Exception)
                        {
                            //ProjectData.SetProjectError(expr_3C);
                            //ProjectData.ClearProjectError();
                        }
                }
            });
        }

        /// <summary>
        ///     获取总内存大小
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double GetTotalPhysicalMemory(Unit unit = Unit.MB)
        {
            var ci = new ComputerInfo();
            double value = ci.TotalPhysicalMemory;
            if (unit == Unit.MB)
                return value / 1024 / 1024;
            if (unit == Unit.GB)
                return value / 1024 / 1024 / 1024;
            if (unit == Unit.KB)
                return value / 1024;
            return value;
        }

        /// <summary>
        ///     获取剩余内存
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double GetAvailablePhysicalMemory(Unit unit = Unit.MB)
        {
            var ci = new ComputerInfo();
            double value = ci.AvailablePhysicalMemory;
            if (unit == Unit.MB)
                return value / 1024 / 1024;
            if (unit == Unit.GB)
                return value / 1024 / 1024 / 1024;
            if (unit == Unit.KB)
                return value / 1024;
            return value;
        }
    }
}