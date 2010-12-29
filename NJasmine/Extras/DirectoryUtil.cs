using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NJasmine.Extras
{
    public static class DirectoryUtil
    {
        public static void DeleteDirectory(string directory)
        {
            const int retries = 15;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    if (Directory.Exists(directory) == false)
                        return;

                    File.SetAttributes(directory, FileAttributes.Normal);
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
                        catch(IOException)
                        {
                        }
                    }

                    Thread.Sleep(100);
                }
            }

            if (Directory.Exists(directory))
                throw new Exception("Could not delete directory " + directory);
        }
    }
}
