using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace TwoThreadWriteApp
{
    public class TwoThreadReadWriteHandler
    {
        private List<DirectoryFiles> _directoryFiles;

        public Thread Thread2;

        public string Copy(string sourceDirectory, string targetDirectory)
        {
            var source = @"\\192.168.150.101\\deploy";
            var target = $"C:\\Users\\amelkin-sa\\OneDrive\\Рабочий стол\\res2";

            var stopWatch = new Stopwatch();
            var diSource = new DirectoryInfo(source);
            var diTarget = new DirectoryInfo(target);
            _directoryFiles = new List<DirectoryFiles>();
            var thread1 = new Thread(() => GetInfo(diSource, diTarget));

            thread1.Start();
            stopWatch.Start();
            while (true)
            {
                if (Thread2 == null)
                {
                    Thread.Sleep(2000);
                    continue;
                }
                if (!Thread2.IsAlive)
                {
                    continue;
                }
                Thread2.Join();
                stopWatch.Stop();
                TimeSpan timeTaken = stopWatch.Elapsed;
                string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
                return foo;
            }

           
            
            _directoryFiles.Clear();
            Thread2 = null;
        }

        public void GetInfo(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var file in source.GetFiles())
            {
                var directoryFiles = new DirectoryFiles()
                {
                    File = file,
                    SubDirectory = target
                };
                //info
                _directoryFiles.Add(directoryFiles);
                if (Thread2 == null)
                {
                    Thread2 = new Thread(WriteFiles);
                    Thread2.Start();
                }
            }

            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                GetInfo(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void WriteFiles()
        {
            try
            {
                int i = 0;
                while (i < _directoryFiles.Count)
                {
                    var directoryFile = _directoryFiles[i];
                    var fileStream = File.ReadAllBytes(directoryFile.File.FullName);
                    File.WriteAllBytes(Path.Combine(directoryFile.SubDirectory.FullName, directoryFile.File.Name), fileStream);
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}