using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using CMFL.MVVM;
using CMFL.MVVM.Class.Helper.Kernel;

/// <summary>
///     P/Invoke封装
/// </summary>
[SecurityCritical]
[SuppressUnmanagedCodeSecurity]
internal static class NativeMethods
{
    #region CMD操作函数封装

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint WinExec(string lpCmdLine, uint uCmdShow);

    #endregion

    #region Windows桌面管理器API封装

    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    #endregion

    #region Windows内存管理函数封装

    [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
    internal static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

    #endregion

    #region GDI图形库API封装

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipCreateEffect(Guid guid, out IntPtr effect);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipDeleteEffect(IntPtr effect);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipGetEffectParameterSize(IntPtr effect, out uint size);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipSetEffectParameters(IntPtr effect, IntPtr parameters, uint size);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipGetEffectParameters(IntPtr effect, ref uint size, IntPtr parameters);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipBitmapApplyEffect(IntPtr bitmap, IntPtr effect, ref Rectangle rectOfInterest,
        bool useAuxData, IntPtr auxData, int auxDataSize);

    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipBitmapCreateApplyEffect(ref IntPtr SrcBitmap, int numInputs, IntPtr effect,
        ref Rectangle rectOfInterest, ref Rectangle outputRect, out IntPtr outputBitmap, bool useAuxData,
        IntPtr auxData, int auxDataSize);

    // 这个函数我在C#下已经调用成功
    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipInitializePalette(IntPtr palette, int palettetype, int optimalColors,
        int useTransparentColor, int bitmap);

    // 该函数一致不成功，不过我在VB6下调用很简单，也很成功，主要是结构体的问题。
    [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipBitmapConvertFormat(IntPtr bitmap, int pixelFormat, int dithertype,
        int palettetype, IntPtr palette, float alphaThresholdPercent);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern bool DeleteObject(IntPtr hObject);

    #endregion

    #region Win32API 托盘函数封装

    /// <summary>
    ///     操作Windows窗体,系统托盘所用的API函数
    /// </summary>
    public const int WM_USER = 0x400;

    public const int WM_CLOSE = 0x10;
    public const int WM_GETTEXT = 0x000D;
    public const int WM_SETTEXT = 0x000C;
    public const int WM_MOUSEMOVE = 0x0200;
    public const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
    public const int SYNCHRONIZE = 0x100000;
    public const int PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF;
    public const int PROCESS_TERMINATE = 0x1;
    public const int PROCESS_VM_OPERATION = 0x8;
    public const int PROCESS_VM_READ = 0x10;
    public const int PROCESS_VM_WRITE = 0x20;
    public const int MEM_RESERVE = 0x2000;
    public const int MEM_COMMIT = 0x1000;
    public const int MEM_RELEASE = 0x8000;
    public const int PAGE_READWRITE = 0x4;
    public const int TB_BUTTONCOUNT = WM_USER + 24;
    public const int TB_HIDEBUTTON = WM_USER + 4;
    public const int TB_GETBUTTON = WM_USER + 23;
    public const int TB_GETBUTTONTEXT = WM_USER + 75;
    public const int TB_GETBITMAP = WM_USER + 44;
    public const int TB_DELETEBUTTON = WM_USER + 22;
    public const int TB_ADDBUTTONS = WM_USER + 20;
    public const int TB_INSERTBUTTON = WM_USER + 21;
    public const int TB_ISBUTTONHIDDEN = WM_USER + 12;
    public const int ILD_NORMAL = 0x0;
    public const int TPM_NONOTIFY = 0x80;
    public const int WS_VISIBLE = 268435456; //窗体可见
    public const int WS_MINIMIZEBOX = 131072; //有最小化按钮
    public const int WS_MAXIMIZEBOX = 65536; //有最大化按钮
    public const int WS_BORDER = 8388608; //窗体有边框
    public const int GWL_STYLE = -16; //窗体样式
    public const int GW_HWNDFIRST = 0;
    public const int GW_HWNDNEXT = 2;
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
    internal static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
    internal static extern void SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll", EntryPoint = "GetDlgItem", SetLastError = true)]
    internal static extern IntPtr GetDlgItem(int nID, IntPtr phWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int RegisterWindowMessage(string msg);

    [DllImport("kernel32", EntryPoint = "OpenProcess")]
    internal static extern IntPtr OpenProcess(int dwDesiredAccess, IntPtr bInheritHandle, IntPtr dwProcessId);

    [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
    internal static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, ref IntPtr lpdwProcessId);

    [DllImport("user32.dll")]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32", EntryPoint = "SendMessage")]
    internal static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

    [DllImport("user32", EntryPoint = "SendMessage")]
    internal static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

    [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
    internal static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer,
        int nSize, int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
    internal static extern bool ReadProcessMemoryEx(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer,
        int nSize,
        ref uint vNumberOfBytesRead);

    [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
    internal static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
        int nSize,
        int lpNumberOfBytesWritten);

    [DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
    internal static extern int WriteProcessMemory(IntPtr hProcess, ref int lpBaseAddress, ref int lpBuffer,
        int nSize,
        ref int lpNumberOfBytesWritten);

    [DllImport("kernel32", EntryPoint = "VirtualAllocEx")]
    internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize,
        int flAllocationType,
        int flProtect);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern int CloseHandle(IntPtr lpObject);

    [DllImport("kernel32", EntryPoint = "VirtualFreeEx")]
    internal static extern int VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);

    [DllImport("User32.dll")]
    internal static extern int GetWindow(int hWnd, int wCmd);

    [DllImport("User32.dll")]
    internal static extern int GetWindowLongA(int hWnd, int wIndx);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

    [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
        string lpszWindow);

    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", EntryPoint = "SendMessageA")]
    internal static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    #endregion
}