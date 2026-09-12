using System.Collections.Generic;

namespace ProCheck
{
    public class NetworkRequestData
    {
        public string Url { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TimeMs { get; set; } = 0;

        public string RemoteIp { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public Dictionary<string, string> RequestHeaders { get; set; } = new();
        public Dictionary<string, string> ResponseHeaders { get; set; } = new();

        public byte[]? UploadedBytes { get; set; }
        public byte[]? DownloadedBytes { get; set; }

        public bool IsComplete { get; set; } = false;
    }
}