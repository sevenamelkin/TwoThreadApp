using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TwoThreadWriteApp
{
    public class TwoThreadReadWriteHandler
    {
        private ListBox _listBox;
        private List<DirectoryFiles> _directoryFiles;
        private Thread _thread2;

        public TwoThreadReadWriteHandler(ListBox listBox)
        {
            _listBox = listBox;
        }

        private void AddItemToListBox(string item)
        {
            _listBox.Items.Add(item);
        }

        public void Copy(string sourceDirectory, string targetDirectory)
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

            _directoryFiles.Clear();
            _thread2 = null;
        }

        private void GetInfo(DirectoryInfo source, DirectoryInfo target)
        {
            int i = 0;
            foreach (var file in source.GetFiles())
            {
                var directoryFiles = new DirectoryFiles()
                {
                    File = file,
                    SubDirectory = target
                };
                var fileName = $"Имя копируемого файла: {file.Name}";
                _listBox.Invoke((Action)(() => _listBox.Items.Add(fileName)));
                _directoryFiles.Add(directoryFiles);
                if (_thread2 == null)
                {
                    _thread2 = new Thread(WriteFiles);
                    _thread2.Start();
                }
            }

            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var catalogName = $"Имя копируемого каталога: {diSourceSubDir.Name}";
                _listBox.Invoke((Action)(() => _listBox.Items.Add(catalogName)));
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                GetInfo(diSourceSubDir, nextTargetSubDir);
            }
        }


        private void WriteFiles()
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