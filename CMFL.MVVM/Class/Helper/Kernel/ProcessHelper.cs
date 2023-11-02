using System;
using System.Diagnostics;

namespace CMFL.MVVM.Class.Helper.Kernel
{
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    /// <summary>
    ///     进程，托盘图标管理方法
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        ///     杀进程
        /// </summary>
        /// <param name="processName"></param>
        public static void KillProcess(string processName)
        {
            //得到所有打开的进程 
            foreach (var proc in Process.GetProcessesByName(processName))
                if (!proc.CloseMainWindow())
                    if (proc != null)
                        proc.Kill();
        }

        /// <summary>
        ///     刷新托盘区域图标
        /// </summary>
        public static void RefreshTrayIcon()
        {
            var systemTrayContainerHandle = NativeMethods.FindWindow("Shell_TrayWnd", null);
            var systemTrayHandle =
                NativeMethods.FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            var sysPagerHandle = NativeMethods.FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            var notificationAreaHandle =
                NativeMethods.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = NativeMethods.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32",
                    "User Promoted Notification Area");
                var notifyIconOverflowWindowHandle = NativeMethods.FindWindow("NotifyIconOverflowWindow", null);
                var overflowNotificationAreaHandle = NativeMethods.FindWindowEx(notifyIconOverflowWindowHandle,
                    IntPtr.Zero,
                    "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }

            RefreshTrayArea(notificationAreaHandle);
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            NativeMethods.GetClientRect(windowHandle, out var rect);
            for (var x = 0; x < rect.Right; x += 5)
            for (var y = 0; y < rect.Bottom; y += 5)
                _ = NativeMethods.SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }

        /// <summary>
        ///     刷新通知栏区域图标
        /// </summary>
        public static void RefreshNotifyIcon()
        {
            var vHandle = NotifyIconOverflowWindow();
            var vProcessId = IntPtr.Zero;
            NativeMethods.GetWindowThreadProcessId(vHandle, ref vProcessId);
            var vProcess =
                NativeMethods.OpenProcess(
                    NativeMethods.PROCESS_VM_OPERATION | NativeMethods.PROCESS_VM_READ |
                    NativeMethods.PROCESS_VM_WRITE, IntPtr.Zero,
                    vProcessId);
            var vPointer = NativeMethods.VirtualAllocEx(vProcess, (int) IntPtr.Zero, 0x1000,
                NativeMethods.MEM_RESERVE | NativeMethods.MEM_COMMIT,
                NativeMethods.PAGE_READWRITE);
            try
            {
                _ = NativeMethods.GetWindowRect(vHandle, out var r);
                var width = r.Right - r.Left;
                var height = r.Bottom - r.Top;
                //从任务栏中间从左到右 MOUSEMOVE一遍，所有图标状态会被更新 
                for (var x = 1; x < width; x++)
                for (var y = 0; y < height; y += 4)
                    _ = NativeMethods.SendMessage(vHandle, NativeMethods.WM_MOUSEMOVE, 0,
                        MakeLParam(x, y));
            }
            finally
            {
                _ = NativeMethods.VirtualFreeEx(vProcess, vPointer, 0, NativeMethods.MEM_RELEASE);
                _ = NativeMethods.CloseHandle(vProcess);
            }
        }

        //获取托盘指针
        private static IntPtr TrayToolbarWindow32()
        {
            var h = NativeMethods.FindWindow("Shell_TrayWnd", null);
            h = NativeMethods.FindWindowEx(h, IntPtr.Zero, "TrayNotifyWnd", null); //找到托盘
            h = NativeMethods.FindWindowEx(h, IntPtr.Zero, "SysPager", null);
            var hTemp = NativeMethods.FindWindowEx(h, IntPtr.Zero, "ToolbarWindow32", null);
            return hTemp;
        }

        private static IntPtr NotifyIconOverflowWindow()
        {
            var h = NativeMethods.FindWindow("NotifyIconOverflowWindow", null); //托盘容器
            var hTemp = NativeMethods.FindWindowEx(h, IntPtr.Zero, "ToolbarWindow32", null);
            return hTemp;
        }

        private static IntPtr MakeLParam(int loWord, int hiWord)
        {
            return (IntPtr) ((hiWord << 16) | (loWord & 0xffff));
        }
    }
}