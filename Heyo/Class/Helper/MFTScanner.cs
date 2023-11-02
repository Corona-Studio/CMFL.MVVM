using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace Heyo.Class.Helper
{
    public class MFTScanner
    {
        private const uint GENERIC_READ = 0x80000000;
        private const int FILE_SHARE_READ = 0x1;
        private const int FILE_SHARE_WRITE = 0x2;
        private const int OPEN_EXISTING = 3;
        private const int FILE_READ_ATTRIBUTES = 0x80;
        private const int FILE_NAME_IINFORMATION = 9;
        private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
        private const int FILE_OPEN_FOR_BACKUP_INTENT = 0x4000;
        private const int FILE_OPEN_BY_FILE_ID = 0x2000;
        private const int FILE_OPEN = 0x1;
        private const int OBJ_CASE_INSENSITIVE = 0x40;
        private const int FSCTL_ENUM_USN_DATA = 0x900b3;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private static IntPtr m_hCJ;
        private static IntPtr m_Buffer;
        private static int m_BufferSize;

        private static string m_DriveLetter;

        private static IntPtr OpenVolume(string szDriveLetter)
        {
            IntPtr hCJ = default;
            //// volume handle

            m_DriveLetter = szDriveLetter;
            hCJ = NativeMethods.CreateFile(@"\\.\" + szDriveLetter, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            return hCJ;
        }


        private static void Cleanup()
        {
            if (m_hCJ != IntPtr.Zero)
            {
                // Close the volume handle.
                NativeMethods.CloseHandle(m_hCJ);
                m_hCJ = INVALID_HANDLE_VALUE;
            }

            if (m_Buffer != IntPtr.Zero)
            {
                // Free the allocated memory
                Marshal.FreeHGlobal(m_Buffer);
                m_Buffer = IntPtr.Zero;
            }
        }


        public static IEnumerable<string> EnumerateFiles(string szDriveLetter, string keyWord)
        {
            try
            {
                var usnRecord = default(NativeMethods.USN_RECORD);
                var mft = default(NativeMethods.MFT_ENUM_DATA);
                var dwRetBytes = 0;
                var cb = 0;
                var dicFRNLookup = new Dictionary<long, FSNode>();
                var bIsFile = false;

                // This shouldn't be called more than once.
                if (m_Buffer.ToInt32() != 0) throw new Exception("invalid buffer");

                // Assign buffer size
                m_BufferSize = 65536;
                //64KB

                // Allocate a buffer to use for reading records.
                m_Buffer = Marshal.AllocHGlobal(m_BufferSize);

                // correct path
                szDriveLetter = szDriveLetter.TrimEnd('\\');

                // Open the volume handle 
                m_hCJ = OpenVolume(szDriveLetter);

                // Check if the volume handle is valid.
                if (m_hCJ == INVALID_HANDLE_VALUE)
                {
                    var errorMsg = "Couldn't open handle to the volume.";
                    if (!PermissionHelper.IsAdministrator())
                        errorMsg += "Current user is not administrator";

                    throw new Exception(errorMsg);
                }

                mft.StartFileReferenceNumber = 0;
                mft.LowUsn = 0;
                mft.HighUsn = long.MaxValue;

                do
                {
                    if (NativeMethods.DeviceIoControl(m_hCJ, FSCTL_ENUM_USN_DATA, ref mft, Marshal.SizeOf(mft),
                        m_Buffer, m_BufferSize, ref dwRetBytes, IntPtr.Zero))
                    {
                        cb = dwRetBytes;
                        // Pointer to the first record
                        var pUsnRecord = new IntPtr(m_Buffer.ToInt32() + 8);

                        while (dwRetBytes > 8)
                        {
                            // Copy pointer to USN_RECORD structure.
                            usnRecord = (NativeMethods.USN_RECORD) Marshal.PtrToStructure(pUsnRecord,
                                usnRecord.GetType());

                            // The filename within the USN_RECORD.
                            var FileName = Marshal.PtrToStringUni(
                                new IntPtr(pUsnRecord.ToInt32() + usnRecord.FileNameOffset),
                                usnRecord.FileNameLength / 2);

                            bIsFile = !usnRecord.FileAttributes.HasFlag(FileAttributes.Directory);
                            dicFRNLookup.Add(usnRecord.FileReferenceNumber,
                                new FSNode(usnRecord.FileReferenceNumber, usnRecord.ParentFileReferenceNumber, FileName,
                                    bIsFile));

                            // Pointer to the next record in the buffer.
                            pUsnRecord = new IntPtr(pUsnRecord.ToInt32() + usnRecord.RecordLength);

                            dwRetBytes -= usnRecord.RecordLength;
                        }

                        // The first 8 bytes is always the start of the next USN.
                        mft.StartFileReferenceNumber = Marshal.ReadInt64(m_Buffer, 0);
                    }
                    else
                    {
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                } while (!(cb <= 8));

                // Resolve all paths for Files
                foreach (var oFSNode in dicFRNLookup.Values.Where(o => o.IsFile && o.FileName.Contains(keyWord)))
                {
                    var sFullPath = oFSNode.FileName;
                    var oParentFSNode = oFSNode;

                    while (dicFRNLookup.TryGetValue(oParentFSNode.ParentFRN, out oParentFSNode))
                        sFullPath = string.Concat(oParentFSNode.FileName, @"\", sFullPath);
                    sFullPath = string.Concat(szDriveLetter, @"\", sFullPath);

                    yield return sFullPath;
                }
            }
            finally
            {
                //// cleanup
                Cleanup();
            }
        }

        public static IEnumerable<string> EnumerateFiles(string keyWord)
        {
            IEnumerable<string> result = new List<string>();
            var selectQuery = new SelectQuery("select * from win32_logicaldisk");
            var searcher = new ManagementObjectSearcher(selectQuery);
            foreach (ManagementObject disk in searcher.Get())
                result = result.Union(EnumerateFiles(disk["Name"].ToString(), keyWord));
            return result;
        }

        public static IEnumerable<string> EnumerateFiles(DriveInfo drive)
        {
            return EnumerateFiles(drive.Name, "");
        }

        private class FSNode
        {
            public readonly string FileName;

            public readonly bool IsFile;
            public readonly long ParentFRN;
            public long FRN;

            public FSNode(long lFRN, long lParentFSN, string sFileName, bool bIsFile)
            {
                FRN = lFRN;
                ParentFRN = lParentFSN;
                FileName = sFileName;
                IsFile = bIsFile;
            }
        }
    }
}