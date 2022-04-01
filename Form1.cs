using System;
using System.IO;
using System.Windows.Forms;

namespace TwoThreadWriteApp
{
    public partial class Form1 : Form
    {
        private TwoThreadReadWriteHandler _readWriteHandler;
        public Form1()
        {
            InitializeComponent();
            _readWriteHandler = new TwoThreadReadWriteHandler();
        }

        private void button1_Click(object sender, EventArgs e)
        { 
           var gg = _readWriteHandler.Copy(textBox1.Text, textBox2.Text);
           textBox1.Text = gg;
        }

        
    }
}
