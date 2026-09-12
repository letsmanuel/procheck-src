using ProCheck;
using SharpDivert;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

        public MonitorScreen(string filePath, Main mainForm)
        {
            InitializeComponent();
            monitorFilePath = filePath;
            this.mainForm = mainForm;
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSuspendProcess(IntPtr processHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtResumeProcess(IntPtr processHandle);

        private void KillMonitoredProcess()
        {
            if (monitoredProcess != null && !monitoredProcess.HasExited)
            {
                try
                {
                    if (isPaused)
                    {
                        NtResumeProcess(monitoredProcess.Handle);
                        isPaused = false;
                    }
                    monitoredProcess.Kill(true);
                }
                catch { }
            }
            monitoredProcess = null;
            isPaused = false;
            pauseButton.Text = "Pause";
        }
        private void GoBackToMain()
        {
            KillMonitoredProcess();
            StopNetworkCapture();
            HttpsCapture.ForceResetSystemProxy();
            mainForm.Show();
            this.Close();
        }

        private async void MonitorScreen_Load(object sender, EventArgs e)
        {
            this.Text = "ProCheck - Monitoring: " + Path.GetFileName(monitorFilePath);

            networkPanel.Enabled = false;
            launchButton.Enabled = false;
            idleToolStripMenuItem.Text = "Verifying WinDivert...";

            bool ok = await WinDivertInstaller.verifyWinDivert();

            if (!ok)
            {
                MessageBox.Show("Failed to install WinDivert. Network monitoring will be unavailable.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                idleToolStripMenuItem.Text = "WinDivert unavailable.";
                return;
            }

            networkPanel.Enabled = true;
            launchButton.Enabled = true;
            idleToolStripMenuItem.Text = "Ready.";

            StartNetworkCapture();
        }

        private void StartNetworkCapture()
        {
            httpsCapture = new HttpsCapture();

            httpsCapture.OnRequestCaptured += (data) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AddNetworkRequestEntry(data)));
                }
                else
                {
                    AddNetworkRequestEntry(data);
                }
            };

            httpsCapture.OnRequestCompleted += (data) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => RefreshNetworkRequestEntry(data)));
                }
                else
                {
                    RefreshNetworkRequestEntry(data);
                }
            };

            bool ok = httpsCapture.Start();
            if (!ok)
            {
                MessageBox.Show("Failed to start network capture.");
            }
        }

        private void StopNetworkCapture()
        {
            httpsCapture?.Stop();
            httpsCapture = null;
            activeRequests.Clear();
            pidTracker?.Stop();
            pidTracker = null;
        }

        private void AddNetworkRequestEntry(NetworkRequestData data)
        {
            var entry = new network_request(data);
            entry.Width = networkPanel.ClientSize.Width - 20;
            networkPanel.Controls.Add(entry);
            activeRequests[data] = entry;
        }

        private void RefreshNetworkRequestEntry(NetworkRequestData data)
        {
            if (activeRequests.TryGetValue(data, out var entry))
            {
                entry.RefreshFromData();
            }
        }

        private void closeApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KillMonitoredProcess();
            StopNetworkCapture();
            HttpsCapture.ForceResetSystemProxy();
            Application.Exit();
        }

        private void exitMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoBackToMain();
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            if (monitoredProcess != null && !monitoredProcess.HasExited)
            {
                MessageBox.Show("A process is already running.");
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = monitorFilePath,
                UseShellExecute = true
            };

            try
            {
                monitoredProcess = Process.Start(psi);
                isPaused = false;

                pidTracker = new PidTreeTracker(monitoredProcess.Id);
                pidTracker.Start();
                httpsCapture?.SetPidTracker(pidTracker);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch: {ex.Message}");
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (monitoredProcess == null || monitoredProcess.HasExited)
            {
                MessageBox.Show("No running process to pause.");
                return;
            }

            if (!isPaused)
            {
                NtSuspendProcess(monitoredProcess.Handle);
                isPaused = true;
                pauseButton.Text = "Resume";
            }
            else
            {
                NtResumeProcess(monitoredProcess.Handle);
                isPaused = false;
                pauseButton.Text = "Pause";
            }
        }

        private void killButton_Click(object sender, EventArgs e)
        {
            if (monitoredProcess == null || monitoredProcess.HasExited)
            {
                MessageBox.Show("No running process to kill.");
                return;
            }

            try
            {
                if (isPaused)
                {
                    NtResumeProcess(monitoredProcess.Handle);
                    isPaused = false;
                }
                monitoredProcess.Kill(true);
                pauseButton.Text = "Pause";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to kill: {ex.Message}");
            }
        }

        private void networkPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void idleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}