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
        private Label _label;
        private int _countFiles;
        private List<DirectoryFiles> _directoryFiles;
        private Thread _thread2;

        public TwoThreadReadWriteHandler(ListBox listBox, Label label)
        {
            _listBox = listBox;
            _label = label;
        }

        public void Copy(string sourceDirectory, string targetDirectory)
        {
            var stopWatch = new Stopwatch();
            var diSource = new DirectoryInfo(sourceDirectory);
            _countFiles = diSource.GetFiles("*.*", SearchOption.AllDirectories).Length;
            var diTarget = new DirectoryInfo(targetDirectory);
            _directoryFiles = new List<DirectoryFiles>();
            var thread1 = new Thread(() => GetInfo(diSource, diTarget));

            thread1.Start();
            stopWatch.Start();

            _directoryFiles.Clear();
            _thread2 = null;
        }

        private void GetInfo(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var file in source.GetFiles())
            {
                var directoryFiles = new DirectoryFiles()
                {
                    File = file,
                    SubDirectory = target
                };
                _directoryFiles.Add(directoryFiles);
                if (_thread2 == null)
                {
                    _thread2 = new Thread(WriteFiles);
                    _thread2.Start();
                }
            }

            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                GetInfo(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void WriteFiles()
        {
            try
            {
                var i = 0;
                while (i < _directoryFiles.Count)
                {
                    var directoryFile = _directoryFiles[i];
                    var message = $"Копируется файл: {directoryFile.File.Name} \nИз папки:{directoryFile.SubDirectory.Name}";
                    _listBox.Invoke((Action)(() => _listBox.Items.Add(message)));
                    var fileStream = File.ReadAllBytes(directoryFile.File.FullName);
                    File.WriteAllBytes(Path.Combine(directoryFile.SubDirectory.FullName, directoryFile.File.Name), fileStream);
                    i++;
                    _label.Invoke((Action)(() => _label.Text = $"{i}/{_countFiles}"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}