using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
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

        public void SetPidTracker(PidTreeTracker? tracker)
        {
            pidTracker = tracker;

            Debug.WriteLine(
                $"[HttpsCapture] PID tracker " +
                $"{(tracker != null ? "attached" : "detached")}.");
        }

        private bool IsPidMonitored(int pid)
        {
            if (pid <= 0)
                return false;

            try
            {
                var tracker = pidTracker;

                if (tracker == null)
                    return false;

                return tracker.IsTracked(pid);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[HttpsCapture] IsPidMonitored({pid}) failed: {ex.Message}");

                return false;
            }
        }

        private bool AnyPidMonitored(
            System.Collections.Generic.IReadOnlyList<int> pids)
        {
            foreach (int pid in pids)
            {
                if (IsPidMonitored(pid))
                    return true;
            }

            return false;
        }

        public bool Start()
        {
            try
            {
                proxyServer = new ProxyServer();

                proxyServer.CertificateManager.EnsureRootCertificate(
                    userTrustRootCertificate: true,
                    machineTrustRootCertificate: false);

                endPoint = new ExplicitProxyEndPoint(
                    IPAddress.Loopback,
                    8501,
                    decryptSsl: true);

                proxyServer.AddEndPoint(endPoint);

                proxyServer.BeforeRequest += OnRequest;
                proxyServer.BeforeResponse += OnResponse;

                proxyServer.Logging.MinimumLevel =
                    LogLevel.Debug;

                proxyServer.Start();

                proxyServer.SetAsSystemHttpProxy(endPoint);
                proxyServer.SetAsSystemHttpsProxy(endPoint);

                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
                AppDomain.CurrentDomain.UnhandledException +=
                    OnUnhandledException;

                Debug.WriteLine(
                    "[HttpsCapture] Proxy started successfully.");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[HttpsCapture] Failed to start: {ex}");

                try
                {
                    ForceResetSystemProxy();
                }
                catch
                {
                }

                return false;
            }
        }

        public void Stop()
        {
            Debug.WriteLine("[HttpsCapture] Stop called.");

            try
            {
                if (proxyServer != null)
                {
                    proxyServer.DisableAllSystemProxies();

                    proxyServer.BeforeRequest -= OnRequest;
                    proxyServer.BeforeResponse -= OnResponse;

                    proxyServer.Stop();
                    proxyServer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[HttpsCapture] Stop exception: {ex.Message}");
            }

            proxyServer = null;
            endPoint = null;
            pidTracker = null;

            ForceResetSystemProxy();
        }

        private async Task OnRequest(
            object sender,
            SessionEventArgs e)
        {
            try
            {
                var request = e.HttpClient.Request;

                int localPort =
                    e.ClientLocalEndPoint?.Port ?? -1;

                var owningPids =
                    localPort > 0
                        ? TcpConnectionOwner.GetOwningPidsForLocalPort(localPort)
                        : Array.Empty<int>();

                bool belongsToMonitoredApp =
                    AnyPidMonitored(owningPids);

                string ownerText =
                    owningPids.Count == 0
                        ? "none"
                        : string.Join(",", owningPids);

                Debug.WriteLine(
                    $"[HttpsCapture] OnRequest: {request.Url} | " +
                    $"localPort={localPort} | " +
                    $"owners=[{ownerText}] | " +
                    $"tracked={belongsToMonitoredApp}");

                if (!belongsToMonitoredApp)
                {
                    Debug.WriteLine(
                        $"[HttpsCapture] Ignored request because " +
                        $"none of the owners are tracked.");

                    return;
                }

                var data = new NetworkRequestData
                {
                    Url = request.Url,
                    Method = request.Method,
                    Status = "Pending"
                };

                foreach (var header in request.Headers)
                {
                    data.RequestHeaders[header.Name] =
                        header.Value;
                }

                if (request.HasBody)
                {
                    try
                    {
                        data.UploadedBytes =
                            await e.GetRequestBody();
                    }
                    catch (Exception bodyEx)
                    {
                        Debug.WriteLine(
                            $"[HttpsCapture] GetRequestBody failed " +
                            $"for {data.Url}: {bodyEx.Message}");
                    }
                }

                e.UserData = data;

                OnRequestCaptured?.Invoke(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[HttpsCapture] OnRequest exception: {ex}");
            }
        }

        private async Task OnResponse(
            object sender,
            SessionEventArgs e)
        {
            try
            {
                if (e.UserData is not NetworkRequestData data)
                    return;

                var response = e.HttpClient.Response;

                data.Status =
                    ((int)response.StatusCode).ToString();

                foreach (var header in response.Headers)
                {
                    data.ResponseHeaders[header.Name] =
                        header.Value;
                }

                if (response.HasBody)
                {
                    try
                    {
                        data.DownloadedBytes =
                            await e.GetResponseBody();
                    }
                    catch (Exception bodyEx)
                    {
                        Debug.WriteLine(
                            $"[HttpsCapture] GetResponseBody failed " +
                            $"for {data.Url}: {bodyEx.Message}");
                    }
                }

                data.IsComplete = true;

                OnRequestCompleted?.Invoke(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[HttpsCapture] OnResponse exception: {ex}");
            }
        }

        private void OnProcessExit(
            object? sender,
            EventArgs e)
        {
            try
            {
                Stop();
            }
            catch
            {
            }
        }

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            try
            {
                Stop();
            }
            catch
            {
            }
        }

        public static void ForceResetSystemProxy()
        {
            try
            {
                using var key =
                    Microsoft.Win32.Registry.CurrentUser
                        .OpenSubKey(
                            @"Software\Microsoft\Windows\CurrentVersion\Internet Settings",
                            writable: true);

                if (key != null)
                {
                    key.SetValue("ProxyEnable", 0);

                    key.DeleteValue(
                        "ProxyServer",
                        throwOnMissingValue: false);

                    key.DeleteValue(
                        "ProxyOverride",
                        throwOnMissingValue: false);
                }

                InternetSetOption(
                    IntPtr.Zero,
                    INTERNET_OPTION_SETTINGS_CHANGED,
                    IntPtr.Zero,
                    0);

                InternetSetOption(
                    IntPtr.Zero,
                    INTERNET_OPTION_REFRESH,
                    IntPtr.Zero,
                    0);
            }
            catch
            {
            }
        }

        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const int INTERNET_OPTION_REFRESH = 37;

        [DllImport("wininet.dll")]
        private static extern bool InternetSetOption(
            IntPtr hInternet,
            int dwOption,
            IntPtr lpBuffer,
            int dwBufferLength);
    }
}