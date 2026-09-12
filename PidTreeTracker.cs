using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ProCheck
{
    internal class PidTreeTracker
    {
        private readonly HashSet<int> watchedPids = new();
        private readonly object lockObj = new();
        private System.Threading.Timer? pollTimer;

        public PidTreeTracker(int rootPid)
        {
            lock (lockObj) watchedPids.Add(rootPid);
        }

        public void Start()
        {
            pollTimer = new System.Threading.Timer(_ => PollForChildren(), null, 0, 500);
        }

        public void Stop()
        {
            pollTimer?.Dispose();
            pollTimer = null;
        }

        public bool IsTracked(int pid)
        {
            lock (lockObj) return watchedPids.Contains(pid);
        }

        private void PollForChildren()
        {
            try
            {
                var processes = Process.GetProcesses();

                List<int> currentlyWatched;
                lock (lockObj) currentlyWatched = watchedPids.ToList();

                foreach (var proc in processes)
                {
                    try
                    {
                        int parentPid = GetParentPid(proc.Id);
                        if (currentlyWatched.Contains(parentPid))
                        {
                            lock (lockObj) watchedPids.Add(proc.Id);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(
            IntPtr processHandle, int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation,
            int processInformationLength, out int returnLength);

        private static int GetParentPid(int pid)
        {
            using var process = Process.GetProcessById(pid);
            var pbi = new PROCESS_BASIC_INFORMATION();
            int status = NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
            if (status != 0) return -1;
            return pbi.InheritedFromUniqueProcessId.ToInt32();
        }
    }
}