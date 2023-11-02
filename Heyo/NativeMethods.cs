using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Heyo
{
    internal static class NativeMethods
    {
        #region MFT函数封装

        [StructLayout(LayoutKind.Sequential)]
        internal struct MFT_ENUM_DATA
        {
            public long StartFileReferenceNumber;
            public long LowUsn;
            public long HighUsn;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USN_RECORD
        {
            public int RecordLength;
            public short MajorVersion;
            public short MinorVersion;
            public long FileReferenceNumber;
            public long ParentFileReferenceNumber;
            public long Usn;
            public long TimeStamp;
            public int Reason;
            public int SourceInfo;
            public int SecurityId;
            public FileAttributes FileAttributes;
            public short FileNameLength;
            public short FileNameOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IO_STATUS_BLOCK
        {
            public int Status;
            public int Information;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct UNICODE_STRING
        {
            public short Length;
            public short MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct OBJECT_ATTRIBUTES
        {
            public int Length;
            public IntPtr RootDirectory;
            public IntPtr ObjectName;
            public int Attributes;
            public int SecurityDescriptor;
            public int SecurityQualityOfService;
        }

        //// MFT_ENUM_DATA
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool DeviceIoControl(IntPtr hDevice, int dwIoControlCode, ref MFT_ENUM_DATA lpInBuffer,
            int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode,
            IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int CloseHandle(IntPtr lpObject);

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int NtCreateFile(ref IntPtr FileHandle, int DesiredAccess,
            ref OBJECT_ATTRIBUTES ObjectAttributes, ref IO_STATUS_BLOCK IoStatusBlock, int AllocationSize,
            int FileAttribs, int SharedAccess, int CreationDisposition, int CreateOptions, int EaBuffer,
            int EaLength);

        [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int NtQueryInformationFile(IntPtr FileHandle, ref IO_STATUS_BLOCK IoStatusBlock,
            IntPtr FileInformation, int Length, int FileInformationClass);

        #endregion

        #region 32位程序读写64注册表

        private static readonly UIntPtr HKEY_CLASSES_ROOT = (UIntPtr) 0x80000000;
        private static readonly UIntPtr HKEY_CURRENT_USER = (UIntPtr) 0x80000001;
        private static readonly UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr) 0x80000002;
        private static readonly UIntPtr HKEY_USERS = (UIntPtr) 0x80000003;
        private static readonly UIntPtr HKEY_CURRENT_CONFIG = (UIntPtr) 0x80000005;

        // 关闭64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        // 开启64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        [DllImport("advapi32.dll")]
        internal static extern int RegQueryInfoKey(
            IntPtr hkey,
            StringBuilder lpClass,
            uint lpcbClass,
            IntPtr lpReserved,
            out uint lpcSubKeys,
            IntPtr lpcbMaxSubKeyLen,
            IntPtr lpcbMaxClassLen,
            out uint lpcValues,
            IntPtr lpcbMaxValueNameLen,
            IntPtr lpcbMaxValueLen,
            IntPtr lpcbSecurityDescriptor,
            IntPtr lpftLastWriteTime
        );

        //枚举指定项下方的子项
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, EntryPoint = "RegEnumKeyEx")]
        internal static extern int RegEnumKeyEx(IntPtr hkey,
            uint index,
            StringBuilder lpName,
            ref uint lpcbName,
            IntPtr reserved,
            IntPtr lpClass,
            IntPtr lpcbClass,
            out long lpftLastWriteTime);

        // 获取操作Key值句柄
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired,
            out IntPtr phkResult);

        //关闭注册表转向（禁用特定项的注册表反射）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern long RegDisableReflectionKey(IntPtr hKey);

        //使能注册表转向（开启特定项的注册表反射）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern long RegEnableReflectionKey(IntPtr hKey);

        //获取Key值（即：Key值句柄所标志的Key对象的值）
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved,
            out uint lpType, StringBuilder lpData,
            ref uint lpcbData);

        public enum ParentKeyName
        {
            HKEY_CLASSES_ROOT,
            HKEY_CURRENT_USER,
            HKEY_LOCAL_MACHINE,
            HKEY_USERS,
            HKEY_CURRENT_CONFIG
        }

        private static UIntPtr TransferKeyName(string keyName)
        {
            switch (keyName)
            {
                case "HKEY_CLASSES_ROOT":
                    return HKEY_CLASSES_ROOT;
                case "HKEY_CURRENT_USER":
                    return HKEY_CURRENT_USER;
                case "HKEY_LOCAL_MACHINE":
                    return HKEY_LOCAL_MACHINE;
                case "HKEY_USERS":
                    return HKEY_USERS;
                case "HKEY_CURRENT_CONFIG":
                    return HKEY_CURRENT_CONFIG;
            }

            return HKEY_CLASSES_ROOT;
        }

        public static string Get64BitRegistryKey(ParentKeyName parentKeyName, string subKeyName, string keyName)
        {
            var KEY_QUERY_VALUE = 0x0001;
            var KEY_WOW64_64KEY = 0x0100;
            var KEY_ALL_WOW64 = KEY_QUERY_VALUE | KEY_WOW64_64KEY;

            try
            {
                //将Windows注册表主键名转化成为不带正负号的整形句柄（与平台是32或者64位有关）
                var hKey = TransferKeyName(parentKeyName.ToString());

                //声明将要获取Key值的句柄
                var pHKey = IntPtr.Zero;

                //记录读取到的Key值
                var result = new StringBuilder("".PadLeft(1024));
                uint resultSize = 1024;
                uint lpType = 0;

                //关闭文件系统转向 
                var oldWOW64State = new IntPtr();
                if (Wow64DisableWow64FsRedirection(ref oldWOW64State))
                {
                    //获得操作Key值的句柄
                    RegOpenKeyEx(hKey, subKeyName, 0, KEY_ALL_WOW64, out pHKey);
                    //关闭注册表转向（禁止特定项的注册表反射）
                    RegDisableReflectionKey(pHKey);
                    //获取访问的Key值
                    RegQueryValueEx(pHKey, keyName, 0, out lpType, result, ref resultSize);
                    //打开注册表转向（开启特定项的注册表反射）
                    RegEnableReflectionKey(pHKey);
                }

                //打开文件系统转向
                Wow64RevertWow64FsRedirection(oldWOW64State);
                //返回Key值
                return result.ToString().Trim();
            }
            catch
            {
                return null;
            }
        }

        public static string[] Enum64BitRegistryKey(ParentKeyName parentKeyName, string subKeyName)
        {
            uint numSubKeys = 0, numValues;
            var hKey = TransferKeyName(parentKeyName.ToString());
            uint s = 0;
            var pHKey = IntPtr.Zero;
            var KEY_QUERY_VALUE = 0x0001;
            var KEY_WOW64_64KEY = 0x0100;
            var KEY_ENUMERATE_SUB_KEYS = 0x0008;
            var KEY_ALL_WOW64 = KEY_QUERY_VALUE | KEY_WOW64_64KEY | KEY_ENUMERATE_SUB_KEYS;
            // Create an array to hold the names.
            var names = new string[0];
            ;

            var oldWOW64State = new IntPtr();
            //关闭文件系统转向 
            if (Wow64DisableWow64FsRedirection(ref oldWOW64State))
            {
                RegOpenKeyEx(hKey, subKeyName, 0, KEY_ALL_WOW64, out pHKey);
                //关闭注册表转向（禁止特定项的注册表反射）
                RegDisableReflectionKey(pHKey);

                RegQueryInfoKey(pHKey, null, s, IntPtr.Zero, out numSubKeys, IntPtr.Zero, IntPtr.Zero, out numValues,
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                names = new string[numSubKeys];
                uint index = 0;
                long writeTime;
                while (index < numSubKeys)
                {
                    uint MAX_REG_KEY_SIZE = 2048;
                    var sb = new StringBuilder();
                    var ret = RegEnumKeyEx(pHKey, index, sb, ref MAX_REG_KEY_SIZE, IntPtr.Zero, IntPtr.Zero,
                        IntPtr.Zero, out writeTime);
                    if (ret != 0) break;
                    Console.WriteLine(sb.ToString());
                    names[(int) index++] = sb.ToString();
                }

                //开启注册表转向
                RegEnableReflectionKey(pHKey);
            }

            //打开文件系统转向
            Wow64RevertWow64FsRedirection(oldWOW64State);

            return names;
        }

        #endregion
    }
}