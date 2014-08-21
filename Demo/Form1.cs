using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
            UHWIDEngine uniqueUIDs = new UHWIDEngine();
            textBox2.Text = uniqueUIDs.SimpleUID;
            textBox1.Text = uniqueUIDs.AdvancedUID;
        }
    }
}
