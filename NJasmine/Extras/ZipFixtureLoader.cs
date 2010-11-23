using System;
using System.Diagnostics;
using System.IO;

namespace NJasmine.Extras
{
    public class ZipFixtureLoader
    {
        public static string  UnzipBinDeployedToTempDirectory(string binRelativePath, string tempName)
        {
            string source = GetAndCheckPathOfBinDeployedFile(binRelativePath);

            return UnzipFileToTempDirectory(source, tempName);
        }

        public static string UnzipFileToTempDirectory(string sourceZipFilepath, string tempName)
        {
            string tempPath = tempPath = Path.Combine(Path.GetTempPath(), tempName);

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            ProcessStartInfo info = new ProcessStartInfo(Get7Zip(),
                                                         String.Format("x -o{1} \"{2}\"", Get7Zip(), tempPath,
                                                                       sourceZipFilepath));
            info.WorkingDirectory = Path.GetTempPath();
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            using (var zipProcess = Process.Start(info))
            {
                string consoleOutput = zipProcess.StandardOutput.ReadToEnd();

                if (!consoleOutput.Contains("Everything is Ok"))
                {
                    Console.WriteLine("7Zip console output:");
                    Console.WriteLine(consoleOutput);
                    Console.WriteLine("7Zip error output:");
                    Console.WriteLine(zipProcess.StandardError.ReadToEnd());

                    throw new Exception("7Zip extraction apparently failed- success message not found.  Actual 7Zip results written to console.");
                }
            }

            return tempPath;
        }

        private static string GetAndCheckPathOfBinDeployedFile(string resourcePath)
        {
            string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourcePath);

            if (!File.Exists(source))
                throw new FileNotFoundException(string.Format("Could not find file at path {0}.", source));
            return source;
        }

        private static string Get7Zip()
        {
            return GetAndCheckPathOfBinDeployedFile("7za.exe");
        }
    }
}
