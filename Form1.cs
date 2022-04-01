using System;
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
           _readWriteHandler.Copy(textBox1.Text, textBox2.Text);
        }
    }
}
