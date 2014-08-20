using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UHWID
{
    public class UniqueHWID
    {
        private string _uID;
        private bool _includeWindows = false;
        public string uID
        {
            get { return _uID; }
            set { _uID = value; }
        }
        public UniqueHWID()
        {
            this._includeWindows = false;
            doUniqueID();
        }
        public UniqueHWID(bool includeWindows)
        {
            this._includeWindows = includeWindows;
            doUniqueID();
        }
        private void doUniqueID()
        {
            string volumeSerial = DiskID.getDiskID();
            string cpuID = CpuID.getCpuID();
            string uniqueID = volumeSerial + cpuID;
            if (this._includeWindows == true)
            {
                string windowsID = WindowsID.getWindowsID();
                uniqueID += windowsID;
            }
            _uID = uniqueID;
        }
    }
    public static class CpuID
    {
        public static string getCpuID()
        {
            return ProcessorId();
        }

        [DllImport("user32", EntryPoint = "CallWindowProcW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr CallWindowProcW([In] byte[] bytes, IntPtr hWnd, int msg, [In, Out] byte[] wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool VirtualProtect([In] byte[] bytes, IntPtr size, int newProtect, out int oldProtect);

        const int PAGE_EXECUTE_READWRITE = 0x40;

        private static string ProcessorId()
        {
            byte[] sn = new byte[8];

            if (!ExecuteCode(ref sn))
                return "ND";

            return string.Format("{0}{1}", BitConverter.ToUInt32(sn, 4).ToString("X8"), BitConverter.ToUInt32(sn, 0).ToString("X8"));
        }

        private static bool ExecuteCode(ref byte[] result)
        {
            int num;

            /* The opcodes below implement a C function with the signature:
             * __stdcall CpuIdWindowProc(hWnd, Msg, wParam, lParam);
             * with wParam interpreted as an 8 byte unsigned character buffer.
             * */

            byte[] code_x86 = new byte[] {
                0x55,                      /* push ebp */
                0x89, 0xe5,                /* mov  ebp, esp */
                0x57,                      /* push edi */
                0x8b, 0x7d, 0x10,          /* mov  edi, [ebp+0x10] */
                0x6a, 0x01,                /* push 0x1 */
                0x58,                      /* pop  eax */
                0x53,                      /* push ebx */
                0x0f, 0xa2,                /* cpuid    */
                0x89, 0x07,                /* mov  [edi], eax */
                0x89, 0x57, 0x04,          /* mov  [edi+0x4], edx */
                0x5b,                      /* pop  ebx */
                0x5f,                      /* pop  edi */
                0x89, 0xec,                /* mov  esp, ebp */
                0x5d,                      /* pop  ebp */
                0xc2, 0x10, 0x00,          /* ret  0x10 */
            };
            byte[] code_x64 = new byte[] {
                0x53,                                     /* push rbx */
                0x48, 0xc7, 0xc0, 0x01, 0x00, 0x00, 0x00, /* mov rax, 0x1 */
                0x0f, 0xa2,                               /* cpuid */
                0x41, 0x89, 0x00,                         /* mov [r8], eax */
                0x41, 0x89, 0x50, 0x04,                   /* mov [r8+0x4], edx */
                0x5b,                                     /* pop rbx */
                0xc3,                                     /* ret */
            };

            if (IsX64Process())
            {
                IntPtr ptr = new IntPtr(code_x64.Length);

                if (!VirtualProtect(code_x64, ptr, PAGE_EXECUTE_READWRITE, out num))
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                ptr = new IntPtr(result.Length);
                return (CallWindowProcW(code_x64, IntPtr.Zero, 0, result, ptr) != IntPtr.Zero);

            }
            else
            {
                IntPtr ptr = new IntPtr(code_x86.Length);

                if (!VirtualProtect(code_x86, ptr, PAGE_EXECUTE_READWRITE, out num))
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                ptr = new IntPtr(result.Length);
                return (CallWindowProcW(code_x86, IntPtr.Zero, 0, result, ptr) != IntPtr.Zero);
            }

        }

        private static bool IsX64Process()
        {
            return IntPtr.Size == 8;
        }
    }
    class DiskID
    {
        public static string getDiskID()
        {
            return DiskId();
        }
        public static string getDiskID(String diskLetter)
        {
            return DiskId(diskLetter);
        }
        private static string DiskId()
        {
            return DiskId("");
        }
        private static string DiskId(String diskLetter)
        {
            //Find first drive
            if (diskLetter == "")
            {
                foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                {
                    if (compDrive.IsReady)
                    {
                        diskLetter = compDrive.RootDirectory.ToString();
                        break;
                    }
                }
            }
            if (diskLetter.EndsWith(":\\"))
            {
                //C:\ -> C
                diskLetter = diskLetter.Substring(0, diskLetter.Length - 2);
            }
            ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + diskLetter + @":""");
            disk.Get();

            string volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }
    }

    class WindowsID
    {
        public static string getWindowsID()
        {
            return WindowsId();
        }
        private static string WindowsId()
        {
            string windowsInfo = "";
            ManagementObjectSearcher managClass = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");

            ManagementObjectCollection managCollec = managClass.Get();

            bool is64bits = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));

            foreach (ManagementObject managObj in managCollec)
            {
                windowsInfo = managObj.Properties["Caption"].Value.ToString() + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432") + managObj.Properties["Version"].Value.ToString();
                break;
            }
            windowsInfo.Replace(" ", "");
            windowsInfo.Replace("Windows", "");
            windowsInfo.Replace("windows", "");
            windowsInfo += (is64bits) ? ":64" : "=32";

            //md5 hash of the windows version
            MD5 md5Hasher = MD5.Create();
            byte[] wi = md5Hasher.ComputeHash(Encoding.Default.GetBytes(windowsInfo));
            string wiHex = BitConverter.ToString(wi);
            wiHex = wiHex.Replace("-", "");

            return wiHex;
        }
    }
   
}
