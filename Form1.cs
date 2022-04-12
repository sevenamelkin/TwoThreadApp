using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TwoThreadWriteApp
{
    public partial class Form1 : Form
    {
        private readonly TwoThreadReadWriteHandler _readWriteHandler;

        public Form1()
        {
            InitializeComponent();
            _readWriteHandler = new TwoThreadReadWriteHandler(listBox1, label3, progressBar1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var source = textBox1.Text;
            var target = textBox2.Text;

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                MessageBox.Show("Не заполнен источник или приёмник", "Ошибка");
                return;
            }

            if (IsValidPath(source) is false || IsValidPath(target) is false)
            {
                MessageBox.Show("Указан неправильный путь", "Ошибка");
                return;
            }

            _readWriteHandler.Copy(textBox1.Text, textBox2.Text);
        }

        private bool IsValidPath(string path)
        {
            Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheck.IsMatch(path.Substring(0, 3))) return false;
            string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
            strTheseAreInvalidFileNameChars += @":/?*" + "\"";
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
            if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
                return false;

            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(path));
            if (!dir.Exists)
                dir.Create();
            return true;
        }
    }
}