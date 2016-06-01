using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Handheld
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.MinimizeBox = false;
            InitializeComponent();

            if (Program.errorMessages != null)
            {
                MessageBox.Show(Program.errorMessages);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuditForm auditForm = new AuditForm();

            auditForm.MinimizeBox = false;
            auditForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RFIDSweep rfidSweepForm = new RFIDSweep();

            rfidSweepForm.MaximizeBox = false;
            rfidSweepForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}