using System;
using System.Diagnostics;
using System.IO;

namespace ProCheck
{
    internal class DefenderScan
    {

        // <summary>
        // Scans a file with Windows Defender and returns a verdict.
        // </summary>
        // <param name="path">The path to the file to scan.</param>
        // <param name="showWindow">If true, also opens a visible window showing the raw scan output, closes after 10s. Verdict is still computed normally in the background.</param>
        // <returns>
        // 0 = File is clean
        // 1 = Threat found
        // 2 = Scan failed
        // 3 = Scan timed out
        // </returns>

        public static int scanFile(string path, out string threatName, bool showWindow = false)
        {
            threatName = null;

            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string mpCmdRunPath = findMpCmdRun();
            #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            // I know its a war crime, but I dont care tbh
            if (mpCmdRunPath == null) return 2;

            if (showWindow)
            {
                var visiblePsi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"\"{mpCmdRunPath}\" -Scan -ScanType 3 -File \"{path}\" -DisableRemediation & timeout /t 10\"",
                    UseShellExecute = true
                };
                Process.Start(visiblePsi);
            }

            var psi = new ProcessStartInfo
            {
                FileName = mpCmdRunPath,
                Arguments = $"-Scan -ScanType 3 -File \"{path}\" -DisableRemediation",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            string stdOut;

            try
            {
                using var process = Process.Start(psi);
                if (process == null) return 2;

                bool finished = process.WaitForExit(120000);
                if (!finished)
                {
                    try { process.Kill(true); } catch { }
                    return 3;
                }

                stdOut = process.StandardOutput.ReadToEnd();
            }
            catch
            {
                return 2;
            }

            if (stdOut.IndexOf("Scan starting", StringComparison.OrdinalIgnoreCase) < 0 ||
                stdOut.IndexOf("Scan finished", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return 2;
            }

            if (stdOut.IndexOf("found no threats", StringComparison.OrdinalIgnoreCase) >= 0)
                return 0;

            if (stdOut.IndexOf("Threat :", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                threatName = extractThreatName(stdOut) ?? "Unknown threat";
                return 1;
            }

            return 2;
        }

        private static string? findMpCmdRun()
        {
            string[] roots =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };

            foreach (var root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                var candidate = Path.Combine(root, "Windows Defender", "MpCmdRun.exe");
                if (File.Exists(candidate)) return candidate;
            }

            return null;
        }

        private static string? extractThreatName(string stdOut)
        {
            const string marker = "Threat :";
            int idx = stdOut.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            int start = idx + marker.Length;
            int end = stdOut.IndexOf('\n', start);
            if (end < 0) end = stdOut.Length;

            return stdOut.Substring(start, end - start).Trim();
        }
    }
}