using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProCheck
{
    internal class Logger
    {

        public static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProCheck", "debug.log");

        public static void Log(string message)
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
            }
        }

    }
}
