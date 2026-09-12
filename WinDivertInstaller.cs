using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProCheck
{
    internal class WinDivertInstaller
    {
        private const string DownloadUrl = "https://reqrypt.org/download/WinDivert-2.2.0-D.zip";
        private const string ZipInnerFolder = "WinDivert-2.2.0-D";

        private static readonly string InstallDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProCheck", "bin");

        private static readonly string DllPath = Path.Combine(InstallDir, "WinDivert.dll");
        private static readonly string Sys64Path = Path.Combine(InstallDir, "WinDivert64.sys");
        private static readonly string Sys32Path = Path.Combine(InstallDir, "WinDivert32.sys");

        public static async Task<bool> verifyWinDivert()
        {
            if (isInstalled()) return true;

            try
            {
                Directory.CreateDirectory(InstallDir);

                string tempZip = Path.Combine(Path.GetTempPath(), $"windivert_{Guid.NewGuid()}.zip");

                using (var http = new HttpClient())
                {
                    byte[] data = await http.GetByteArrayAsync(DownloadUrl);
                    await File.WriteAllBytesAsync(tempZip, data);
                }

                string tempExtractDir = Path.Combine(Path.GetTempPath(), $"windivert_extract_{Guid.NewGuid()}");
                ZipFile.ExtractToDirectory(tempZip, tempExtractDir);

                string sourceFolder = Path.Combine(tempExtractDir, ZipInnerFolder);

                CopyIfExists(Path.Combine(sourceFolder, "x64", "WinDivert.dll"), DllPath);
                CopyIfExists(Path.Combine(sourceFolder, "x64", "WinDivert64.sys"), Sys64Path);
                CopyIfExists(Path.Combine(sourceFolder, "x86", "WinDivert32.sys"), Sys32Path);

                try { File.Delete(tempZip); } catch { }
                try { Directory.Delete(tempExtractDir, true); } catch { }
            }
            catch
            {
                return false;
            }

            return isInstalled();
        }

        private static bool isInstalled()
        {
            return File.Exists(DllPath) && File.Exists(Sys64Path);
        }

        private static void CopyIfExists(string source, string destination)
        {
            if (File.Exists(source))
            {
                File.Copy(source, destination, overwrite: true);
            }
        }

        public static string GetInstallDir() => InstallDir;
    }
}