namespace Handheld
{
    partial class AuditForm
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
            this.barcodeLabel = new System.Windows.Forms.Label();
            this.tagList = new System.Windows.Forms.TextBox();

            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Return";
            this.button1.Click += new System.EventHandler(this.returnButton);
            // 
            // barcodeLabel
            // 
            this.barcodeLabel.Location = new System.Drawing.Point(3, 0);
            this.barcodeLabel.Name = "barcodeLabel";
            this.barcodeLabel.Size = new System.Drawing.Size(234, 20);
            this.barcodeLabel.Text = "Scan Barcode";
            this.barcodeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tagList
            // 
            this.tagList.Location = new System.Drawing.Point(3, 23);
            this.tagList.Multiline = true;
            this.tagList.Name = "tagList";
            this.tagList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tagList.Size = new System.Drawing.Size(234, 133);
            this.tagList.TabIndex = 2;
            this.tagList.WordWrap = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(29, 162);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 32);
            this.button2.TabIndex = 4;
            this.button2.Text = "Clear";
            this.button2.Click += new System.EventHandler(this.clearScreenButton);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(138, 162);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 32);
            this.button3.TabIndex = 5;
            this.button3.Text = "TIDs";
            this.button3.Click += new System.EventHandler(this.tidButton);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(138, 214);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 20);
            this.button4.TabIndex = 7;
            this.button4.Text = "Exception";
            this.button4.Click += new System.EventHandler(this.exceptionButton_Click);
            // 
            // AuditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tagList);
            this.Controls.Add(this.barcodeLabel);
            this.Controls.Add(this.button1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "AuditForm";
            this.Text = "AuditForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label barcodeLabel;
        private System.Windows.Forms.TextBox tagList;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}