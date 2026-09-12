using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace ProCheck
{
    internal class HttpsCapture
    {
        private ProxyServer? proxyServer;
        private ExplicitProxyEndPoint? endPoint;
        private PidTreeTracker? pidTracker;

        public event Action<NetworkRequestData>? OnRequestCaptured;
        public event Action<NetworkRequestData>? OnRequestCompleted;

        public void SetPidTracker(PidTreeTracker tracker)
        {
            pidTracker = tracker;
        }

        public bool Start()
        {
            try
            {
                proxyServer = new ProxyServer();

                proxyServer.CertificateManager.EnsureRootCertificate(
                    userTrustRootCertificate: true,
                    machineTrustRootCertificate: false);

                endPoint = new ExplicitProxyEndPoint(IPAddress.Loopback, 8501, decryptSsl: true);
                proxyServer.AddEndPoint(endPoint);

                proxyServer.BeforeRequest += OnRequest;
                proxyServer.BeforeResponse += OnResponse;

                proxyServer.Logging.MinimumLevel = LogLevel.Debug;

                proxyServer.Start();

                proxyServer.SetAsSystemHttpProxy(endPoint);
                proxyServer.SetAsSystemHttpsProxy(endPoint);

                AppDomain.CurrentDomain.ProcessExit += (s, e) => Stop();
                AppDomain.CurrentDomain.UnhandledException += (s, e) => Stop();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to start HTTPS capture: {ex.Message}");
                ForceResetSystemProxy();
                return false;
            }
        }

        public void Stop()
        {
            if (proxyServer == null) return;

            try
            {
                proxyServer.DisableAllSystemProxies();
                proxyServer.BeforeRequest -= OnRequest;
                proxyServer.BeforeResponse -= OnResponse;
                proxyServer.Stop();
                proxyServer.Dispose();
            }
            catch { }

            proxyServer = null;

            ForceResetSystemProxy();
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            var request = e.HttpClient.Request;

            bool belongsToMonitoredApp = true;
            
            /*
            try
            {
                int localPort = e.ClientLocalEndPoint?.Port ?? -1;
                if (localPort >= 0 && pidTracker != null)
                {
                    int owningPid = TcpConnectionOwner.GetOwningPidForLocalPort(localPort);
                    belongsToMonitoredApp = owningPid >= 0 && pidTracker.IsTracked(owningPid);
                }
            }
            catch
            {
                belongsToMonitoredApp = false;
            }
            */

            var data = new NetworkRequestData
            {
                Url = request.Url,
                Method = request.Method,
                Status = "Pending"
            };

            foreach (var header in request.Headers)
                data.RequestHeaders[header.Name] = header.Value;

            if (request.HasBody)
            {
                try { data.UploadedBytes = await e.GetRequestBody(); }
                catch { }
            }

            e.UserData = data;

            if (belongsToMonitoredApp)
                OnRequestCaptured?.Invoke(data); 
        }
        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            if (e.UserData is not NetworkRequestData data) return;

            var response = e.HttpClient.Response;

            data.Status = ((int)response.StatusCode).ToString();

            foreach (var header in response.Headers)
            {
                data.ResponseHeaders[header.Name] = header.Value;
            }

            if (response.HasBody)
            {
                try
                {
                    data.DownloadedBytes = await e.GetResponseBody();
                }
                catch { }
            }

            data.IsComplete = true;

            OnRequestCompleted?.Invoke(data);
        }

        // <summary>
        // Forcibly clears Windows' system proxy settings, regardless of whether
        // this instance's own proxy server is running. Safe to call at any time,
        // including at app startup to clean up after a crashed previous run.
        // </summary>
        public static void ForceResetSystemProxy()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Internet Settings", writable: true);

                if (key != null)
                {
                    key.SetValue("ProxyEnable", 0);
                    key.DeleteValue("ProxyServer", throwOnMissingValue: false);
                    key.DeleteValue("ProxyOverride", throwOnMissingValue: false);
                }

                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            }
            catch { }
        }

        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const int INTERNET_OPTION_REFRESH = 37;

        [DllImport("wininet.dll")]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
    }
}