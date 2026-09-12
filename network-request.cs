using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProCheck
{
    public partial class network_request : UserControl
    {
        public NetworkRequestData Data { get; private set; }

        public network_request(NetworkRequestData data)
        {
            InitializeComponent();
            Data = data;
            RefreshDisplay();
        }

        static string Shorten(string text)
        {
            return text.Length > 25
                ? text.Substring(0, 22) + "..."
                : text;
        }

        public network_request(string url, string method) : this(new NetworkRequestData
        {
            Url = url,
            Method = method,
            Status = "Pending"
        })
        { }

        private void RefreshDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshDisplay));
                return;
            }

            methodLabel.Text = Data.Method;
            statusLabel.Text = Data.Status;
            timeLabel.Text = Data.TimeMs.ToString() + "ms";
            urlLabel.Text = Shorten(Data.Url);
        }

        public void RefreshFromData()
        {
            RefreshDisplay();
        }

        public void UpdateElapsedTime(int elapsedMs)
        {
            Data.TimeMs = elapsedMs;

            if (InvokeRequired)
            {
                Invoke(new Action(() => timeLabel.Text = elapsedMs.ToString() + "ms"));
            }
            else
            {
                timeLabel.Text = elapsedMs.ToString() + "ms";
            }
        }

        public void CompleteRequest(
            string status,
            int finalTimeMs,
            string remoteIp,
            string country,
            Dictionary<string, string> requestHeaders,
            Dictionary<string, string> responseHeaders,
            byte[]? uploadedBytes,
            byte[]? downloadedBytes)
        {
            Data.Status = status;
            Data.TimeMs = finalTimeMs;
            Data.RemoteIp = remoteIp;
            Data.Country = country;
            Data.RequestHeaders = requestHeaders ?? new();
            Data.ResponseHeaders = responseHeaders ?? new();
            Data.UploadedBytes = uploadedBytes;
            Data.DownloadedBytes = downloadedBytes;
            Data.IsComplete = true;

            RefreshDisplay();
        }

        private void network_request_Load(object sender, EventArgs e)
        {

        }

        private void copyLink_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Data.Url);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello World (print!)");
        }
    }
}