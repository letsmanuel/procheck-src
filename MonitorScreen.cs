using ProCheck;
using SharpDivert;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProCheck
{
    public partial class MonitorScreen : Form
    {
        private Main mainForm;
        public string monitorFilePath = string.Empty;

        private Process? monitoredProcess;
        private bool isPaused = false;

        private HttpsCapture? httpsCapture;
        private PidTreeTracker? pidTracker;
        private readonly Dictionary<NetworkRequestData, network_request> activeRequests = new();

        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProCheck", "debug.log");

        public MonitorScreen(string filePath, Main mainForm)
        {
            InitializeComponent();

            monitorFilePath = filePath;
            this.mainForm = mainForm;

            Log($"MonitorScreen constructed. Instance hash: {this.GetHashCode()}, filePath: {filePath}");
        }

        // Writes a timestamped line to both Debug Output and a persistent log file.
        private static void Log(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
            Debug.WriteLine(line);

            try
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(LogFilePath)!);

                File.AppendAllText(
                    LogFilePath,
                    line + Environment.NewLine);
            }
            catch
            {
                // Logging must never crash the app.
            }
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSuspendProcess(
            IntPtr processHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtResumeProcess(
            IntPtr processHandle);

        // ------------------------------------------------------------
        // PROCESS TREE HELPERS
        // ------------------------------------------------------------

        private bool IsMonitoredApplicationRunning()
        {
            if (pidTracker != null)
            {
                bool running = pidTracker.HasRunningProcess();

                Log($"IsMonitoredApplicationRunning: tracker says {running}");

                return running;
            }

            if (monitoredProcess != null)
            {
                bool running = !TryHasExited(monitoredProcess);

                Log($"IsMonitoredApplicationRunning: fallback root process says {running}");

                return running;
            }

            return false;
        }

        private static bool TryHasExited(Process process)
        {
            try
            {
                return process.HasExited;
            }
            catch
            {
                return true;
            }
        }

        private void LogTrackedProcesses()
        {
            if (pidTracker == null)
            {
                Log("No PidTreeTracker available.");
                return;
            }

            try
            {
                IReadOnlyCollection<int> pids =
                    pidTracker.GetTrackedPids();

                Log($"Currently tracked PIDs: {string.Join(", ", pids)}");
            }
            catch (Exception ex)
            {
                Log($"Failed to enumerate tracked PIDs: {ex}");
            }
        }

        // ------------------------------------------------------------
        // KILL ENTIRE TRACKED PROCESS TREE
        // ------------------------------------------------------------

        private void KillMonitoredProcess()
        {
            Log("KillMonitoredProcess called.");

            try
            {
                if (pidTracker != null)
                {
                    IReadOnlyCollection<int> trackedPids =
                        pidTracker.GetTrackedPids();

                    Log($"Attempting to kill {trackedPids.Count} tracked processes.");

                    // Take a snapshot because processes may disappear
                    // while we're killing the tree.
                    foreach (int pid in trackedPids.Reverse())
                    {
                        try
                        {
                            using Process process =
                                Process.GetProcessById(pid);

                            if (process.HasExited)
                                continue;

                            try
                            {
                                if (isPaused)
                                {
                                    NtResumeProcess(process.Handle);
                                }
                            }
                            catch
                            {
                                // Process may already be gone.
                            }

                            process.Kill(true);

                            Log($"Killed tracked process PID {pid}.");
                        }
                        catch (ArgumentException)
                        {
                            Log($"PID {pid} no longer exists.");
                        }
                        catch (InvalidOperationException)
                        {
                            Log($"PID {pid} was already terminated.");
                        }
                        catch (Exception ex)
                        {
                            Log($"Failed to kill PID {pid}: {ex}");
                        }
                    }
                }
                else if (monitoredProcess != null)
                {
                    // Fallback if the tracker could not be created.
                    try
                    {
                        if (!monitoredProcess.HasExited)
                        {
                            if (isPaused)
                            {
                                NtResumeProcess(
                                    monitoredProcess.Handle);

                                isPaused = false;
                            }

                            monitoredProcess.Kill(true);

                            Log("Fallback Process.Kill(true) succeeded.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Fallback kill failed: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"EXCEPTION in KillMonitoredProcess: {ex}");
            }

            isPaused = false;

            try
            {
                pidTracker?.Stop();
            }
            catch
            {
            }

            pidTracker = null;
            monitoredProcess = null;

            pauseButton.Text = "Pause";

            Log("KillMonitoredProcess finished.");
        }

        // ------------------------------------------------------------
        // NAVIGATION
        // ------------------------------------------------------------

        private void GoBackToMain()
        {
            Log("GoBackToMain called.");

            KillMonitoredProcess();
            StopNetworkCapture();
            HttpsCapture.ForceResetSystemProxy();

            mainForm.Show();
            this.Close();
        }

        // ------------------------------------------------------------
        // FORM LOAD
        // ------------------------------------------------------------

        private async void MonitorScreen_Load(
            object sender,
            EventArgs e)
        {
            Log($"MonitorScreen_Load started. Instance hash: {this.GetHashCode()}");

            this.Text =
                "ProCheck - Monitoring: " +
                Path.GetFileName(monitorFilePath);

            networkPanel.Enabled = false;
            launchButton.Enabled = false;

            idleToolStripMenuItem.Text =
                "Verifying WinDivert...";

            bool ok;

            try
            {
                ok = await WinDivertInstaller.verifyWinDivert();
            }
            catch (Exception ex)
            {
                Log($"EXCEPTION during verifyWinDivert: {ex}");
                ok = false;
            }

            Log($"WinDivert verification result: {ok}");

            if (!ok)
            {
                MessageBox.Show(
                    "Failed to install WinDivert. Network monitoring will be unavailable.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                idleToolStripMenuItem.Text =
                    "WinDivert unavailable.";

                return;
            }

            networkPanel.Enabled = true;
            launchButton.Enabled = true;

            idleToolStripMenuItem.Text = "Ready.";

            Log(
                "MonitorScreen_Load finished, " +
                "launchButton.Enabled = true. " +
                "Starting network capture.");

            StartNetworkCapture();
        }

        // ------------------------------------------------------------
        // NETWORK CAPTURE
        // ------------------------------------------------------------

        private void StartNetworkCapture()
        {
            Log("StartNetworkCapture called.");

            httpsCapture = new HttpsCapture();

            httpsCapture.OnRequestCaptured += (data) =>
            {
                Log($"OnRequestCaptured fired: {data.Url}");

                if (InvokeRequired)
                {
                    Invoke(
                        new Action(
                            () => AddNetworkRequestEntry(data)));
                }
                else
                {
                    AddNetworkRequestEntry(data);
                }
            };

            httpsCapture.OnRequestCompleted += (data) =>
            {
                Log(
                    $"OnRequestCompleted fired: {data.Url}, " +
                    $"status: {data.Status}");

                if (InvokeRequired)
                {
                    Invoke(
                        new Action(
                            () => RefreshNetworkRequestEntry(data)));
                }
                else
                {
                    RefreshNetworkRequestEntry(data);
                }
            };

            bool ok = httpsCapture.Start();

            Log($"httpsCapture.Start() result: {ok}");

            if (!ok)
            {
                MessageBox.Show(
                    "Failed to start network capture.");
            }
        }

        private void StopNetworkCapture()
        {
            Log("StopNetworkCapture called.");

            httpsCapture?.Stop();
            httpsCapture = null;

            activeRequests.Clear();

            pidTracker?.Stop();
            pidTracker = null;
        }

        private void AddNetworkRequestEntry(
            NetworkRequestData data)
        {
            Log($"AddNetworkRequestEntry: {data.Url}");

            var entry = new network_request(data);

            entry.Width =
                networkPanel.ClientSize.Width - 20;

            networkPanel.Controls.Add(entry);

            activeRequests[data] = entry;
        }

        private void RefreshNetworkRequestEntry(
            NetworkRequestData data)
        {
            if (activeRequests.TryGetValue(
                data,
                out var entry))
            {
                entry.RefreshFromData();

                Log(
                    $"RefreshNetworkRequestEntry matched and refreshed: " +
                    $"{data.Url}");
            }
            else
            {
                Log(
                    $"RefreshNetworkRequestEntry: NO MATCH FOUND " +
                    $"for {data.Url} -- entry was never added or " +
                    $"dictionary key mismatch");
            }
        }

        // ------------------------------------------------------------
        // MENU EVENTS
        // ------------------------------------------------------------

        private void closeApplicationToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Log(
                "closeApplicationToolStripMenuItem_Click called.");

            KillMonitoredProcess();
            StopNetworkCapture();
            HttpsCapture.ForceResetSystemProxy();

            Application.Exit();
        }

        private void exitMonitorToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Log(
                "exitMonitorToolStripMenuItem_Click called.");

            GoBackToMain();
        }

        // ------------------------------------------------------------
        // LAUNCH
        // ------------------------------------------------------------

        private void launchButton_Click(
            object sender,
            EventArgs e)
        {
            Log(
                $"launchButton_Click called. " +
                $"Instance hash: {this.GetHashCode()}, " +
                $"monitorFilePath: {monitorFilePath}, " +
                $"launchButton.Enabled: {launchButton.Enabled}");

            // IMPORTANT:
            // Do NOT use monitoredProcess.HasExited here.
            //
            // The original process may exit while a child process
            // continues running.
            if (IsMonitoredApplicationRunning())
            {
                Log(
                    "Guard triggered: a tracked process is already running.");

                LogTrackedProcesses();

                MessageBox.Show(
                    "A monitored application is already running.");

                return;
            }

            if (string.IsNullOrEmpty(monitorFilePath) ||
                !File.Exists(monitorFilePath))
            {
                Log(
                    $"ABORT: monitorFilePath is invalid or missing " +
                    $"on disk: '{monitorFilePath}'");

                MessageBox.Show(
                    $"Cannot launch -- file not found: {monitorFilePath}");

                return;
            }

            // Clean up an old tracker if the previous application
            // completely disappeared.
            pidTracker?.Stop();
            pidTracker = null;

            monitoredProcess = null;
            isPaused = false;

            var psi = new ProcessStartInfo
            {
                FileName = monitorFilePath,

                // Keep your original behavior.
                UseShellExecute = true
            };

            try
            {
                monitoredProcess = Process.Start(psi);

                if (monitoredProcess == null)
                {
                    Log(
                        "CRITICAL: Process.Start returned null " +
                        "with no exception thrown.");

                    MessageBox.Show(
                        "Launch failed silently -- Process.Start returned null.");

                    return;
                }

                Log(
                    $"Process.Start succeeded. " +
                    $"PID: {monitoredProcess.Id}, " +
                    $"HasExited immediately after start: " +
                    $"{TryHasExited(monitoredProcess)}");

                isPaused = false;

                // Create the tracker immediately.
                //
                // The tracker should also enumerate already-existing
                // descendants, so it doesn't matter if the child was
                // created extremely quickly.
                try
                {
                    pidTracker =
                        new PidTreeTracker(
                            monitoredProcess.Id);

                    pidTracker.Start();

                    httpsCapture?.SetPidTracker(
                        pidTracker);

                    Log(
                        "PidTreeTracker created and started successfully.");

                    LogTrackedProcesses();
                }
                catch (Exception trackerEx)
                {
                    Log(
                        $"EXCEPTION setting up PidTreeTracker: " +
                        $"{trackerEx}");

                    pidTracker = null;

                    // Launch itself still succeeded.
                }
            }
            catch (Exception ex)
            {
                Log(
                    $"EXCEPTION in Process.Start: {ex}");

                monitoredProcess = null;

                MessageBox.Show(
                    $"Failed to launch: {ex.Message}");
            }

            Log(
                $"launchButton_Click finished. " +
                $"monitoredProcess is now: " +
                $"{(monitoredProcess != null ? monitoredProcess.Id.ToString() : "null")}");
        }

        // ------------------------------------------------------------
        // PAUSE / RESUME
        // ------------------------------------------------------------

        private void pauseButton_Click(
            object sender,
            EventArgs e)
        {
            Log(
                $"pauseButton_Click called. " +
                $"monitoredProcess null: {monitoredProcess == null}, " +
                $"tree running: {IsMonitoredApplicationRunning()}");

            if (!IsMonitoredApplicationRunning())
            {
                MessageBox.Show(
                    "No running monitored process.");

                return;
            }

            if (pidTracker == null)
            {
                MessageBox.Show(
                    "Process tracker is unavailable.");

                return;
            }

            try
            {
                IReadOnlyCollection<int> trackedPids =
                    pidTracker.GetTrackedPids();

                if (!isPaused)
                {
                    int suspended = 0;

                    foreach (int pid in trackedPids)
                    {
                        try
                        {
                            using Process process =
                                Process.GetProcessById(pid);

                            if (process.HasExited)
                                continue;

                            int result =
                                NtSuspendProcess(
                                    process.Handle);

                            Log(
                                $"NtSuspendProcess PID {pid}: " +
                                $"result={result}");

                            suspended++;
                        }
                        catch (Exception ex)
                        {
                            Log(
                                $"Failed to suspend PID {pid}: {ex}");
                        }
                    }

                    if (suspended > 0)
                    {
                        isPaused = true;
                        pauseButton.Text = "Resume";

                        Log(
                            $"Suspended {suspended} tracked processes.");
                    }
                }
                else
                {
                    int resumed = 0;

                    foreach (int pid in trackedPids)
                    {
                        try
                        {
                            using Process process =
                                Process.GetProcessById(pid);

                            if (process.HasExited)
                                continue;

                            int result =
                                NtResumeProcess(
                                    process.Handle);

                            Log(
                                $"NtResumeProcess PID {pid}: " +
                                $"result={result}");

                            resumed++;
                        }
                        catch (Exception ex)
                        {
                            Log(
                                $"Failed to resume PID {pid}: {ex}");
                        }
                    }

                    isPaused = false;
                    pauseButton.Text = "Pause";

                    Log(
                        $"Resumed {resumed} tracked processes.");
                }
            }
            catch (Exception ex)
            {
                Log(
                    $"EXCEPTION in pauseButton_Click: {ex}");

                MessageBox.Show(
                    $"Failed to change process state: {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // KILL
        // ------------------------------------------------------------

        private void killButton_Click(
            object sender,
            EventArgs e)
        {
            Log(
                $"killButton_Click called. " +
                $"monitoredProcess null: {monitoredProcess == null}, " +
                $"tree running: {IsMonitoredApplicationRunning()}");

            if (!IsMonitoredApplicationRunning())
            {
                MessageBox.Show(
                    "No running monitored process.");

                return;
            }

            KillMonitoredProcess();
        }

        // ------------------------------------------------------------
        // OTHER EVENTS
        // ------------------------------------------------------------

        private void networkPanel_Paint(
            object sender,
            PaintEventArgs e)
        {
        }

        private void idleToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            string monitoredInfo =
                monitoredProcess != null
                    ? $"Root PID {monitoredProcess.Id}, " +
                      $"HasExited: {TryHasExited(monitoredProcess)}"
                    : "null";

            string trackedInfo = "none";

            if (pidTracker != null)
            {
                try
                {
                    var pids =
                        pidTracker.GetTrackedPids();

                    trackedInfo =
                        string.Join(", ", pids);
                }
                catch
                {
                    trackedInfo = "error";
                }
            }

            string status =
                $"Instance hash: {this.GetHashCode()}\n" +
                $"monitorFilePath: {monitorFilePath}\n" +
                $"monitoredProcess: {monitoredInfo}\n" +
                $"tracked PIDs: {trackedInfo}\n" +
                $"tree running: {IsMonitoredApplicationRunning()}\n" +
                $"isPaused: {isPaused}\n" +
                $"httpsCapture: {(httpsCapture != null ? "running" : "null")}\n" +
                $"pidTracker: {(pidTracker != null ? "running" : "null")}\n" +
                $"activeRequests count: {activeRequests.Count}\n" +
                $"launchButton.Enabled: {launchButton.Enabled}\n" +
                $"networkPanel.Enabled: {networkPanel.Enabled}\n" +
                $"Log file: {LogFilePath}";

            Log(
                "Debug status requested:\n" +
                status);

            MessageBox.Show(
                status,
                "ProCheck Debug Status");
        }
    }
}
