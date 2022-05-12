using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Labs_OS3a
{
    public partial class Form2 : Form
    {
        
        private struct PROCESS_MEMORY_COUNTERS
        {
            public uint cb;
            public uint PageFaultCount;
            public UInt64 PeakWorkingSetSize;
            public UInt64 WorkingSetSize;
            public UInt64 QuotaPeakPagedPoolUsage;
            public UInt64 QuotaPagedPoolUsage;
            public UInt64 QuotaPeakNonPagedPoolUsage;
            public UInt64 QuotaNonPagedPoolUsage;
            public UInt64 PagefileUsage;
            public UInt64 PeakPagefileUsage;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };
        public class CreateToolhelp32SnapshotFlags

        {

            public const uint TH32CS_SNAPHEAPLIST = 0x00000001;
            public const uint TH32CS_SNAPPROCESS = 0x00000002;
            public const uint TH32CS_SNAPTHREAD = 0x00000004;
            public const uint TH32CS_SNAPMODULE = 0x00000008;
            public const uint TH32CS_SNAPMODULE32 = 0x00000010;
            public const uint TH32CS_SNAPALL = (TH32CS_SNAPHEAPLIST | TH32CS_SNAPPROCESS | TH32CS_SNAPTHREAD | TH32CS_SNAPMODULE);
            public const uint TH32CS_INHERIT = 0x80000000;
        }
        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            Inherit = 0x80000000,
            NoHeaps = 0x40000000

        }

        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);
        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess((uint)flags, false, (uint)proc.Id);
        }
        [DllImport("kernel32.dll", SetLastError = false)]
        static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);
        public Form2()
        {
            InitializeComponent();
            int amount_of_threads = 0;
            int amount_of_process = 0;
            IntPtr h;
            PROCESS_MEMORY_COUNTERS pmc;
            int cb;
            IntPtr hSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process, 0);
            PROCESSENTRY32 lppe = new PROCESSENTRY32();
            lppe.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
            if (hSnapshot.Equals(0))
            {
                Close();
            }
            if (Process32First(hSnapshot, ref lppe))
            {
                amount_of_process++;
                Encoding CN = Encoding.Unicode;
                Encoding unicode = Encoding.ASCII;
                string fn = Path.GetFileName(lppe.szExeFile);
                byte[] unicodeBytes = CN.GetBytes(fn);
                string CNString = unicode.GetString(unicodeBytes);
                dataGridView1.Rows.Add(amount_of_process.ToString(), CNString, lppe.th32ProcessID.ToString(), ((int)lppe.cntThreads).ToString(), "Unknown");
                amount_of_threads += (int)lppe.cntThreads;
                h = OpenProcess(0x00000400, false, lppe.th32ProcessID);
                if ((int)h != 0)
                {
                    cb = (int)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
                    pmc = new PROCESS_MEMORY_COUNTERS();
                    pmc.cb = (uint)cb;
                    if (GetProcessMemoryInfo(h, out pmc, (uint)cb))
                    {
                        dataGridView1.Rows[(int)amount_of_process - 1].Cells[4].Value = (pmc.WorkingSetSize / 1024 ).ToString() + " kb";
                    }
                }
                while (Process32Next(hSnapshot, ref lppe))
                {
                    amount_of_process++;
                    string fn_ = Path.GetFileName(lppe.szExeFile);
                    byte[] unicodeBytes_;
                    unicodeBytes_ = CN.GetBytes(fn_);
                    string CNString_ = unicode.GetString(unicodeBytes_);
                    string n = "";
                    for (int i = 0; i < CNString_.Length; i++)
                    {
                        if (CNString_[i] == '\0')
                        {
                            break;
                        }
                        n += CNString_[i];
                    }
                    dataGridView1.Rows.Add(amount_of_process.ToString(), n, lppe.th32ProcessID.ToString(), ((int)lppe.cntThreads).ToString(), "Unknown");
                    amount_of_threads += (int)lppe.cntThreads;
                    h = OpenProcess(0x00000400, false, lppe.th32ProcessID);
                    if ((int)h != 0)
                    {
                        cb = (int)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
                        pmc = new PROCESS_MEMORY_COUNTERS();
                        pmc.cb = (uint)cb;
                        if (GetProcessMemoryInfo(h, out pmc, (uint)cb))
                        {
                            dataGridView1.Rows[(int)amount_of_process - 1].Cells[4].Value = (pmc.WorkingSetSize / 1024 ).ToString() + " kb";
                        }
                    }
                }
                label1.Text = "Всего потоков: " + amount_of_threads.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(UInt32.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString()));
            f.ShowDialog();
        }
    }
}
