using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PeNet;
using PeNet.Header.Pe;

namespace ProCheck
{
    internal class ValidateFileSafety
    {

        // <summary>
        // Checks if the file at the given path is safe to use.
        // </summary>
        // <param name="path">The path to the file to check.</param>
        // <returns>
        // 0 = File is safe
        // 1 = File does not exist
        // 2 = File is not safe
        // 3 = File is not an executable
        // 4 = File is not a valid PE file
        // 5 = Unknown error
        // 6 = Defender error
        // </returns>

        public static int isFileSafe(string path, bool bypassDefender)
        {
            if (!File.Exists(path)) return 1;
            Console.WriteLine("Starting file safety check for: " + path);

            int defenderResult = 0;
            string threatName = string.Empty;
            if (!bypassDefender) {
                defenderResult = DefenderScan.scanFile(path, out threatName, false);
            }
            Console.WriteLine("Windows Defender scan result: " + defenderResult);

            if (defenderResult.Equals(2))
            {
                Console.WriteLine($"File is not safe. Threat: {threatName}");
                return 2;
            }

            if (!defenderResult.Equals(0))
            {
                Console.WriteLine($"Defender scan failed with code {defenderResult}.");
                return 6;
            }

            PeFile pe;
            try
            {
                pe = new PeFile(path);
            }
            catch
            {
                return 4;
            }

            if (pe.ImageNtHeaders == null) return 4;

            bool isExecutable = (pe.ImageNtHeaders.FileHeader.Characteristics
                & FileCharacteristicsType.ExecutableImage) != 0;
            if (!isExecutable) return 3;

            return 0;
        }

        public static bool isFileExecutable(string path)
        {
            if (!File.Exists(path)) return false;
            PeFile pe;
            try
            {
                pe = new PeFile(path);
            }
            catch
            {
                return false;
            }
            if (pe.ImageNtHeaders == null) return false;
            bool isExecutable = (pe.ImageNtHeaders.FileHeader.Characteristics
                & FileCharacteristicsType.ExecutableImage) != 0;
            return isExecutable;
        }

        public static bool isFileValidPE(string path)
        {
            if (!File.Exists(path)) return false;
            PeFile pe;
            try
            {
                pe = new PeFile(path);
            }
            catch
            {
                return false;
            }
            return pe.ImageNtHeaders != null;
        }

        public static string? getFileThreatName(string path)
        {
            int defenderResult = DefenderScan.scanFile(path, out string threatName, false);
            if (defenderResult.Equals(2))
            {
                return threatName;
            }
            return null;
        }


    }
}