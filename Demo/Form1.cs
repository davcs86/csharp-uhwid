using System;
using System.Windows.Forms;
using UHWID;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = UHWIDEngine.SimpleUid;
            textBox1.Text = UHWIDEngine.AdvancedUid;
        }
    }
}
