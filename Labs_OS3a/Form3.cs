using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Labs_OS3a
{
    public partial class Form3 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal ushort processorArchitecture;
            internal ushort reserved;
            internal uint pageSize;
            internal IntPtr minimumApplicationAddress;
            internal IntPtr maximumApplicationAddress;
            internal IntPtr activeProcessorMask;
            internal uint numberOfProcessors;
            internal uint processorType;
            internal uint allocationGranularity;
            internal ushort processorLevel;
            internal ushort processorRevision;
        }
        [DllImport("kernel32.dll", SetLastError = true)]

        internal static extern void GetSystemInfo(ref SYSTEM_INFO Info);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
             uint processAccess,
             bool bInheritHandle,
             uint processId
        );
        public enum AllocationProtectEnum : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }
        public enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum TypeEnum : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtectEnum AllocationProtect;
            public IntPtr RegionSize;
            public StateEnum State;
            public AllocationProtectEnum Protect;
            public TypeEnum Type;
        }

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, uint lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        public Form3(uint i)
        {
            InitializeComponent();
            IntPtr h;
            SYSTEM_INFO lpSystemInfo = new SYSTEM_INFO();
            MEMORY_BASIC_INFORMATION lpBuffer = new MEMORY_BASIC_INFORMATION();
            GetSystemInfo(ref lpSystemInfo);
            uint add = (uint)lpSystemInfo.minimumApplicationAddress;
            uint dwLength = (uint)Marshal.SizeOf(lpBuffer);
            h = OpenProcess(0x00000400, false, i);
            dataGridView1.Columns[1].HeaderText = i.ToString();
            dataGridView1.Columns[0].HeaderText = (lpBuffer.RegionSize).ToString();
            dataGridView1.Columns[2].HeaderText = ((uint)add).ToString() + " " + ((uint)lpSystemInfo.maximumApplicationAddress).ToString();
            do
            {
                int indexRow = dataGridView1.Rows.Add();
                VirtualQueryEx(h, add, out lpBuffer, dwLength);
                dataGridView1[0, indexRow].Value = lpBuffer.BaseAddress.ToString("X6");
                switch (lpBuffer.Type)
                {
                    case TypeEnum.MEM_IMAGE:
                        dataGridView1[1, indexRow].Value = "Image";
                        break;
                    case TypeEnum.MEM_MAPPED:
                        dataGridView1[1, indexRow].Value = "Mapped";
                        break;
                    case TypeEnum.MEM_PRIVATE:
                        dataGridView1[1, indexRow].Value = "Private";
                        break;
                    default:
                        dataGridView1[1, indexRow].Value = "Unknown";
                        break;
                }
                dataGridView1[2, indexRow].Value = lpBuffer.RegionSize.ToInt32() / 1024;
                switch (lpBuffer.State)
                {
                    case StateEnum.MEM_COMMIT:
                        dataGridView1[3, indexRow].Value = "Commit";
                        break;
                    case StateEnum.MEM_FREE:
                        dataGridView1[3, indexRow].Value = "Free";
                        break;
                    case StateEnum.MEM_RESERVE:
                        dataGridView1[3, indexRow].Value = "Reserve";
                        break;
                    default:
                        dataGridView1[3, indexRow].Value = "Unknown";
                        break;
                }
                switch (lpBuffer.Protect)
                {
                    case AllocationProtectEnum.PAGE_EXECUTE:
                        dataGridView1[4, indexRow].Value = "Execute";
                        break;
                    case AllocationProtectEnum.PAGE_EXECUTE_READ:
                        dataGridView1[4, indexRow].Value = "Execute_Read";
                        break;
                    case AllocationProtectEnum.PAGE_EXECUTE_READWRITE:
                        dataGridView1[4, indexRow].Value = "Execute_ReadWrite";
                        break;
                    case AllocationProtectEnum.PAGE_EXECUTE_WRITECOPY:
                        dataGridView1[4, indexRow].Value = "Execute_WriteCopy";
                        break;
                    case AllocationProtectEnum.PAGE_GUARD:
                        dataGridView1[4, indexRow].Value = "Guard";
                        break;
                    case AllocationProtectEnum.PAGE_NOACCESS:
                        dataGridView1[4, indexRow].Value = "NoAccess";
                        break;
                    case AllocationProtectEnum.PAGE_NOCACHE:
                        dataGridView1[4, indexRow].Value = "NoCache";
                        break;
                    case AllocationProtectEnum.PAGE_READONLY:
                        dataGridView1[4, indexRow].Value = "ReadOnly";
                        break;
                    case AllocationProtectEnum.PAGE_READWRITE:
                        dataGridView1[4, indexRow].Value = "ReadWrite";
                        break;
                    case AllocationProtectEnum.PAGE_WRITECOMBINE:
                        dataGridView1[4, indexRow].Value = "WriteCombine";
                        break;
                    case AllocationProtectEnum.PAGE_WRITECOPY:
                        dataGridView1[4, indexRow].Value = "WriteCopy";
                        break;
                    default:
                        dataGridView1[4, indexRow].Value = "Unknown";
                        break;
                }
                add += (uint)lpBuffer.RegionSize;
            } while (add <= (uint)lpSystemInfo.maximumApplicationAddress);
        }
    }
}
