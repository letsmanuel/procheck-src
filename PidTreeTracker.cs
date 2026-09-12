using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Linq;

namespace ProCheck
{
    internal class PidTreeTracker
    {
        private readonly HashSet<int> watchedPids = new();
        private readonly object lockObj = new();

        private ManagementEventWatcher? startWatcher;
        private ManagementEventWatcher? stopWatcher;

        public PidTreeTracker(int rootPid)
        {
            lock (lockObj)
            {
                watchedPids.Add(rootPid);

                // Important:
                // Capture descendants that may already exist before
                // the WMI start watcher was started.
                AddExistingDescendants(rootPid);
            }
        }

        public void Start()
        {
            try
            {
                startWatcher = new ManagementEventWatcher(
                    new WqlEventQuery(
                        "SELECT * FROM Win32_ProcessStartTrace"));

                startWatcher.EventArrived += OnProcessStarted;
                startWatcher.Start();

                stopWatcher = new ManagementEventWatcher(
                    new WqlEventQuery(
                        "SELECT * FROM Win32_ProcessStopTrace"));

                stopWatcher.EventArrived += OnProcessStopped;
                stopWatcher.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"PidTreeTracker.Start failed: {ex}");
            }
        }

        public void Stop()
        {
            try
            {
                startWatcher?.Stop();
                startWatcher?.Dispose();
            }
            catch { }

            try
            {
                stopWatcher?.Stop();
                stopWatcher?.Dispose();
            }
            catch { }

            startWatcher = null;
            stopWatcher = null;
        }

        public bool IsTracked(int pid)
        {
            lock (lockObj)
            {
                return watchedPids.Contains(pid);
            }
        }

        public bool HasRunningProcess()
        {
            int[] pids;

            lock (lockObj)
            {
                pids = watchedPids.ToArray();
            }

            foreach (int pid in pids)
            {
                try
                {
                    using Process process = Process.GetProcessById(pid);

                    if (!process.HasExited)
                        return true;
                }
                catch
                {
                    // Process disappeared between enumeration and lookup.
                }
            }

            return false;
        }

        public IReadOnlyCollection<int> GetTrackedPids()
        {
            lock (lockObj)
            {
                return watchedPids.ToArray();
            }
        }

        private void AddExistingDescendants(int rootPid)
        {
            try
            {
                bool changed;

                do
                {
                    changed = false;

                    using ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher(
                            "SELECT ProcessId, ParentProcessId " +
                            "FROM Win32_Process");

                    foreach (ManagementObject process in searcher.Get())
                    {
                        try
                        {
                            int pid = Convert.ToInt32(
                                process["ProcessId"]);

                            int parentPid = Convert.ToInt32(
                                process["ParentProcessId"]);

                            if (watchedPids.Contains(parentPid) &&
                                watchedPids.Add(pid))
                            {
                                changed = true;
                            }
                        }
                        catch
                        {
                            // Ignore processes that disappear while
                            // enumerating the WMI result.
                        }
                    }
                }
                while (changed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"AddExistingDescendants failed: {ex}");
            }
        }

        private void OnProcessStarted(
            object sender,
            EventArrivedEventArgs e)
        {
            try
            {
                int newPid = Convert.ToInt32(
                    e.NewEvent.Properties["ProcessID"].Value);

                int parentPid = Convert.ToInt32(
                    e.NewEvent.Properties["ParentProcessID"].Value);

                lock (lockObj)
                {
                    if (watchedPids.Contains(parentPid))
                    {
                        watchedPids.Add(newPid);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"OnProcessStarted failed: {ex}");
            }
        }

        private void OnProcessStopped(
            object sender,
            EventArrivedEventArgs e)
        {
            try
            {
                int pid = Convert.ToInt32(
                    e.NewEvent.Properties["ProcessID"].Value);

                lock (lockObj)
                {
                    watchedPids.Remove(pid);
                }
            }
            catch
            {
            }
        }
    }
}