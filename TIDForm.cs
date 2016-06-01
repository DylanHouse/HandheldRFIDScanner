using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace Handheld
{
    public partial class TIDForm : Form
    {

        public int position;

        public TIDForm()
        {

            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

        }

        public TIDForm(int p, RFIDTag t, Dictionary<string, string[][]> tidObject) : this()
        {
            
            position = p;

            int xPos = 0;
            int yPos = 5;

            Program.debugFile.WriteLine("TID: {0}",t.TID);
            Program.debugFile.WriteLine("Short TID: {0}", t.TID.Substring(0,8));

            string nonSerialTID = t.TID.Substring(0, 8);

            try
            {
                Point topLeftCorner;

                // Substring first eight to remove the serialization portion of the TID
                for (int i = 0; i < tidObject[nonSerialTID][0].Length; i++)
                {
                    try
                    {
                        PictureBox button = new PictureBox();
                        button.Image = (Image)new Bitmap("\\Program Files\\handheld\\images\\" + (tidObject[nonSerialTID][1][i]).ToString());
                        button.Width = button.Image.Width;
                        button.Height = button.Image.Height;

                        button.BackColor = Color.Gray;

                        button.Name = (tidObject[nonSerialTID][0][i]).ToString();

                        button.Click += new System.EventHandler(this.button_Click);

                        topLeftCorner = new Point(((this.Size.Width - button.Width) / 2), yPos);

                        button.Location = topLeftCorner;
                        yPos += button.Height + 10;

                        //Program.debugFile.WriteLine("Image {0} Position : ({1},{2})", i, button.Location.X, button.Location.Y);
                        //Program.debugFile.WriteLine("Image {0} Size : ({1},{2})", i, button.Height, button.Width);

                        this.Controls.Add(button);

                        //Program.debugFile.WriteLine("{0}: {1}", i, button.Name);
                    }
                    catch (Exception e)
                    {
                        Program.debugFile.WriteLine("Image Load Error: " + e.Message);
                    }
                }
                Button noButton = new Button();
                noButton.Text = "Unavailable";
                noButton.Click += new System.EventHandler(this.button_Click);
                noButton.Width = noButton.Width * 4;
                noButton.Height = noButton.Height * 2;

                topLeftCorner = new Point(((this.Size.Width - noButton.Width) / 2), ((this.Size.Height - noButton.Height)));

                noButton.Location = topLeftCorner;

                this.Controls.Add(noButton);

            }
            catch (KeyNotFoundException knfe)
            {
                Button button = new Button();
                button.Text = "No Inlay Selections Available";
                button.Click += new System.EventHandler(this.button_Click);
                button.Width = button.Width * 4;
                button.Height = button.Height * 2;

                Point topLeftCorner = new Point(((this.Size.Width - button.Width) / 2), ((this.Size.Height - button.Height) / 2));

                button.Location = topLeftCorner;


                this.Controls.Add(button);
            }
        }

        private void button_Click(Object sender, System.EventArgs e)
        {
            Program.debugFile.WriteLine("Type of Button: {0}",sender.GetType().ToString());

            if (sender.GetType().ToString() == "System.Windows.Forms.PictureBox")
            {
                PictureBox button = (PictureBox)sender;
                if (MessageBox.Show(button.Name, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    AuditForm.outputObj.tags.ToArray()[position].tidModel = button.Name;

                    this.Dispose();
                    this.Hide();
                }
            }
            else
            {
                this.Dispose();
                this.Hide();
            }
        }
    }
}