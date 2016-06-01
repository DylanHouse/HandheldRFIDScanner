using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol.ResourceCoordination;
using Symbol.RFID3;

namespace Handheld
{
    public partial class SystemForm : Form
    {
        public SystemForm()
        {
            InitializeComponent();

            /*SystemInfo s = new SystemInfo();

            this.textBox1.Text = "RAM: " + s.RAMAvailable + "\r\n";
            this.textBox1.Text = this.textBox1.Text + "LOC: " + s.ReaderLocation + "\r\n";
            this.textBox1.Text = this.textBox1.Text + "FMW: " + s.RadioFirmwareVersion + "\r\n";
            this.textBox1.Text = this.textBox1.Text + "NAM: " + s.ReaderName + "\r\n";
            this.textBox1.Text = this.textBox1.Text + "FSH: " + s.FlashAvailable;*/

       }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}