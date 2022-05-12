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

namespace Labs_OS3a
{
    public partial class Form1 : Form
    {


        
        //
        // Alternate, Structure Version.  This One Allows Correct Marshalling As A "ref" Parameter,
        // But You Have To Ensure The Value Of dwLength Gets Set Correctly Via A Wrapper Method.
        //
        /// <summary>Contains information about the current state of both physical and virtual
        /// memory, including extended memory. The GlobalMemoryStatusEx function stores
        /// information in this structure.</summary>
        /// <remarks>MEMORYSTATUSEX reflects the state of memory at the time of the call. It also
        /// reflects the size of the paging file at that time. The operating system can enlarge
        /// the paging file up to the maximum size set by the administrator.</remarks>
        //[StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            /// <summary>The size of the structure, in bytes. You must set this member before
            /// calling GlobalMemoryStatusEx.</summary>
            public uint dwLength;
            /// <summary>A number between 0 and 100 that specifies the approximate percentage
            /// of physical memory that is in use (0 indicates no memory use and 100
            /// indicates full memory use).</summary>
            public uint dwMemoryLoad;
            /// <summary>The amount of actual physical memory, in bytes.</summary>
            public ulong ullTotalPhys;
            /// <summary>The amount of physical memory currently available, in bytes. This is the
            /// amount of physical memory that can be immediately reused without having to write
            /// its contents to disk first. It is the sum of the size of the standby, free, and
            /// zero lists.</summary>
            public ulong ullAvailPhys;
            /// <summary>The current committed memory limit for the system or the current process,
            /// whichever is smaller, in bytes. To get the system-wide committed memory limit,
            /// call GetPerformanceInfo.</summary>
            public ulong ullTotalPageFile;
            /// <summary>The maximum amount of memory the current process can commit, in bytes.
            /// This value is equal to or smaller than the system-wide available commit value.
            /// To calculate the system-wide available commit value, call GetPerformanceInfo
            /// and subtract the value of CommitTotal from the value of CommitLimit.</summary>
            public ulong ullAvailPageFile;
            /// <summary>The size of the user-mode portion of the virtual address space of the
            /// calling process, in bytes. This value depends on the type of process, the type
            /// of processor, and the configuration of the operating system. For example, this
            /// value is approximately 2 GB for most 32-bit processes on an x86 processor and
            /// approximately 3 GB for 32-bit processes that are large address aware running
            /// on a system with 4-gigabyte tuning enabled.</summary>
            public ulong ullTotalVirtual;
            /// <summary>The amount of unreserved and uncommitted memory currently in the user-mode
            /// portion of the virtual address space of the calling process, in bytes.</summary>
            public ulong ullAvailVirtual;
            /// <summary>Reserved. This value is always 0.</summary>
            public ulong ullAvailExtendedVirtual;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer); //Used to use ref with comment below
                                                                                    // but ref doesn't work.(Use of [In, Out] instead of ref
                                                                                    //causes access violation exception on windows xp
                                                                                    //comment: most probably caused by MEMORYSTATUSEX being declared as a class
                                                                                    //(at least at pinvoke.net). On Win7, ref and struct work.

        // Alternate Version Using "ref," And Works With Alternate Code Below.
        // Also See Alternate Version Of [MEMORYSTATUSEX] Defined As A Structure.
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GlobalMemoryStatusEx", SetLastError = true)]
        static extern bool _GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);


        public Form1()
        {
            
            InitializeComponent();
            MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
            unsafe
            {
                ms.dwLength = (uint)sizeof(MEMORYSTATUSEX);
            }
            _GlobalMemoryStatusEx(ref ms);
            label1.Text = (ms.ullTotalPhys / 1024 / 1024).ToString();
            label2.Text = (ms.ullAvailPhys / 1024 / 1024).ToString();
            label3.Text = ms.dwMemoryLoad.ToString();

            label4.Text = (ms.ullTotalVirtual/ 1024 / 1024).ToString();
            label5.Text = (ms.ullAvailVirtual / 1024 / 1024).ToString();
            label6.Text = ((ms.ullTotalVirtual / 1024 / 1024 - ms.ullAvailVirtual / 1024 / 1024) * 100 / (ms.ullTotalVirtual / 1024 / 1024)).ToString();

            label7.Text = (ms.ullTotalPageFile / 1024 / 1024).ToString();
            label8.Text = (ms.ullAvailPageFile / 1024 / 1024).ToString();
            label9.Text = ((ms.ullTotalPageFile / 1024 / 1024 - ms.ullAvailPageFile / 1024 / 1024) * 100 / (ms.ullTotalPageFile / 1024 / 1024)).ToString();

            //chart3.Series.Clear();
            //chart3.Series.Add("% загрузки");
            //chart3.Series.Add("% доступно");
            chart1.Series["% загрузки"].Points.Add(double.Parse(label3.Text));
            chart1.Series["% доступно"].Points.Add(100 - double.Parse(label3.Text));

            chart2.Series["% загрузки"].Points.Add(double.Parse(label6.Text));
            chart2.Series["% доступно"].Points.Add(100 - double.Parse(label6.Text));

            chart3.Series["% загрузки"].Points.Add(double.Parse(label9.Text));
            chart3.Series["% доступно"].Points.Add(100 - double.Parse(label9.Text));


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }
    }
}
