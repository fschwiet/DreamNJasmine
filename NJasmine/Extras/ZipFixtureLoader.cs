using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

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
            string tempPath = Path.Combine(Path.GetTempPath(), tempName);

            DeleteDirectory(tempPath);
            Directory.CreateDirectory(tempPath);
            
            ZipInputStream s = new ZipInputStream(File.OpenRead(sourceZipFilepath));
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = tempPath;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();


            //string tempPath = tempPath = Path.Combine(Path.GetTempPath(), tempName);

            //DeleteDirectory(tempPath);

            //ProcessStartInfo info = new ProcessStartInfo(Get7Zip(),
            //                                             String.Format("x -o{1} \"{2}\"", Get7Zip(), tempPath,
            //                                                           sourceZipFilepath));
            //info.WorkingDirectory = Path.GetTempPath();
            //info.UseShellExecute = false;
            //info.CreateNoWindow = true;
            //info.RedirectStandardOutput = true;
            //info.RedirectStandardError = true;

            //using (var zipProcess = Process.Start(info))
            //{
            //    string consoleOutput = zipProcess.StandardOutput.ReadToEnd();

            //    if (!consoleOutput.Contains("Everything is Ok"))
            //    {
            //        Console.WriteLine("7Zip console output:");
            //        Console.WriteLine(consoleOutput);
            //        Console.WriteLine("7Zip error output:");
            //        Console.WriteLine(zipProcess.StandardError.ReadToEnd());

            //        throw new Exception("7Zip extraction apparently failed- success message not found.  Actual 7Zip results written to console.");
            //    }
            //}

            return tempPath;
        }

        public static string GetAndCheckPathOfBinDeployedFile(string resourcePath)
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

        public static void DeleteDirectory(string directory)
        {
            const int retries = 10;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    if (Directory.Exists(directory) == false)
                        return;

                    try
                    {
                        File.SetAttributes(directory, FileAttributes.Normal);
                    }
                    catch (IOException)
                    {
                    }
                    Directory.Delete(directory, true);
                    return;
                }
                catch (IOException)
                {
                    foreach (var childDir in Directory.GetDirectories(directory))
                    {
                        try
                        {
                            File.SetAttributes(childDir, FileAttributes.Normal);
                        }
                        catch (IOException)
                        {
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }
    }
}
