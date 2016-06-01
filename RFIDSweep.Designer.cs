namespace Handheld
{
    partial class RFIDSweep
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        //private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.tagList = new System.Windows.Forms.TextBox();
            this.countLabel = new System.Windows.Forms.Label();
            this.counter = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Return";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tagList
            // 
            this.tagList.Location = new System.Drawing.Point(36, 34);
            this.tagList.Multiline = true;
            this.tagList.Name = "tagList";
            this.tagList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tagList.Size = new System.Drawing.Size(171, 143);
            this.tagList.TabIndex = 1;
            // 
            // countLabel
            // 
            this.countLabel.Location = new System.Drawing.Point(36, 11);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(66, 20);
            this.countLabel.Text = "Tag Count:";
            // 
            // counter
            // 
            this.counter.Location = new System.Drawing.Point(108, 11);
            this.counter.Name = "counter";
            this.counter.Size = new System.Drawing.Size(100, 20);
            // 
            // comboBox1
            // 
            this.comboBox1.Location = new System.Drawing.Point(23, 209);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(100, 22);
            this.comboBox1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(129, 211);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Set Session";
            this.button2.Click += new System.EventHandler(this.sessionSetButton);
            // 
            // RFIDSweep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.tagList);
            this.Controls.Add(this.button1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "RFIDSweep";
            this.Text = "RFIDSweep";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tagList;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label counter;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button2;
    }
}