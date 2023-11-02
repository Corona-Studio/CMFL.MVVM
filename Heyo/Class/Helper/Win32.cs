using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Point = System.Windows.Point;

namespace Heyo.Class.Helper
{
    public static class Win32
    {
        [DllImport("shell32.DLL", EntryPoint = "ExtractAssociatedIcon")]
        private static extern int ExtractAssociatedIconA(int hInst, string lpIconPath, ref int lpiIcon);

        public static Icon GetIcon(string path)
        {
            IntPtr thisHandle;
            var RefInt = 0;
            thisHandle = new IntPtr(ExtractAssociatedIconA(0, path, ref RefInt));
            return Icon.FromHandle(thisHandle);
        }

        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public POINT(Point pt)
            {
                X = Convert.ToInt32(pt.X);
                Y = Convert.ToInt32(pt.Y);
            }
        }
    }
}