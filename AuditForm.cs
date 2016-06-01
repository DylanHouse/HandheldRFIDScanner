using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol.ResourceCoordination;
using System.Text.RegularExpressions;

using Symbol.RFID3;


namespace Handheld
{
    public partial class AuditForm : Form
    {
        public static int pullCount = 0;
        public static Dictionary<string, bool> cycleItems = null;
        public static Dictionary<string, string[][]> tidOptions = null;

        public static string barcode = "";

        public static OutputData outputObj;

        public AuditForm()
        {
            // Need to see if the cycle count picked up all of the items.
            // The file cycleCount.txt should be loaded and compared after each RF capture

            cycleItems = new Dictionary<string, bool>();
            tidOptions = new Dictionary<string, string[][]>();

            try
            {
                System.IO.StreamReader fileReader = new System.IO.StreamReader("\\Program Files\\handheld\\cycleCount.txt");
                string line;                

                while ((line = fileReader.ReadLine()) != null)
                {
                    try
                    {
                        // The reader module returns Upper Case so we ToUpper() everything:
                        cycleItems.Add((new Regex(@"^[A-F0-9]{24}$")).Matches(line.ToUpper())[0].Captures[0].Value, true);

                    }
                    catch (Exception regExEx)
                    {
                        Program.debugFile.WriteLine("ERROR:{0}\r\n{1}", regExEx.Message, regExEx.StackTrace);
                    }
                }

                fileReader.Close();

                //Reading in the TID matrix for identification later
                //
                // File Input Looks like:
                //
                //  TID,{model-1:model-2:...:model-n},{imageFileName-1:imageFile-2:...:imageFile-n}
                //

                fileReader = new System.IO.StreamReader("\\Program Files\\handheld\\TIDs.txt");

                while ((line = fileReader.ReadLine()) != null)
                {
                    Program.debugFile.WriteLine("Line: {0}", line);
                    try
                    {
                        string[] lines = line.Split(',');
                        Program.debugFile.WriteLine("TID: {0}", lines[0]);

                        for (int i = 0; i < 2; i++)
                        {
                            lines[i+1] = Regex.Replace(lines[i+1], @"^{", "");
                            Program.debugFile.WriteLine("Skip: 3");
                            lines[i+1] = Regex.Replace(lines[i+1], @"}$", "");
                            Program.debugFile.WriteLine("Skip: 4");
                        }

                        string[] models = lines[1].Split(':');
                        
                        foreach (String m in models)
                        {
                            Program.debugFile.WriteLine("Model: {0}", m);
                        }
                        
                        string[] imgs = lines[2].Split(':');

                        foreach (String g in imgs)
                        {
                            Program.debugFile.WriteLine("Model: {0}", g);
                        }

                        tidOptions.Add(lines[0], new string[][] {models, imgs});
                        Program.debugFile.WriteLine("Skip: 7");

                    }
                    catch (Exception regExEx)
                    {
                        Program.debugFile.WriteLine("TID ERROR:{0}\r\n{1}", regExEx.Message, regExEx.StackTrace);
                    }
                }
                fileReader.Close();
            }
            catch (Exception cycleCountFile)
            {
                Program.debugFile.WriteLine("ERROR:{0}\r\n{1}", cycleCountFile.Message, cycleCountFile.StackTrace);
            }

            this.MinimizeBox = false;
            InitializeComponent();
            Program.setAntennaPower(5);

            //First scan a barcode
            try
            {
                Program.readerTrigger.Stage2Notify += new Trigger.TriggerEventHandler(BarcodeScanEvent);
            }
            catch (Exception e)
            {
                Program.debugFile.WriteLine(e.Message);
            }
        }

        void RFIDScanEvent(object sender, TriggerEventArgs EvtArgs)
        {
            Symbol.RFID3.TagData[] remainingTags = null;
            
            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(RFIDScanEvent); 
                Program.readerRFID.Actions.TagAccess.OperationSequence.PerformSequence();

                while (Program.readerTrigger.State != TriggerState.RELEASED)
                {    
                    System.Threading.Thread.Sleep(10);
                }

                Program.readerRFID.Actions.TagAccess.OperationSequence.StopSequence();

                try
                {
                    remainingTags = Program.readerRFID.Actions.GetReadTags(500);

                    for (int i = 0; i < remainingTags.Length; i++)
                    {
                        bool skipThisLoop = false;

                        try
                        {
                            string tagId = remainingTags[i].TagID;
                            string tagTID = remainingTags[i].MemoryBankData;

                            for (int j = 0; j < outputObj.tags.ToArray().Length; j++)
                            {
                                //Program.debugFile.WriteLine("{0}\r\n{1}", outputObj.tags.ToArray()[j].hexValue, tagId);
                                if (outputObj.tags.ToArray()[j].hexValue.ToLower() == tagId.ToLower())
                                {
                                    skipThisLoop = true;
                                }
                            }

                            if (skipThisLoop)
                            {
                                continue;
                            }

                            //Program.debugFile.WriteLine("{0}:{1}", i, tagId);

                            RFIDTag tag = new RFIDTag();
                            tag.hexValue = tagId;
                            tag.TID = tagTID;
                            
                            //Tesing if the cycle count file contains this tag id


                            TextBox tagTextBox = new TextBox();
                            
                            try
                            {
                                tag = new RFIDTag(tagId, tagTID);                           

                                if(Regex.Replace(barcode, @"^0+", "") == Regex.Replace(tag.barcode, @"^0+", ""))
                                {
                                    tag.barcodeMismatch = false;

                                    //tagTextBox.Text = "EPC: " + tagId + "\r\n" + "TID: " + tagTID + "\r\n" + "UPC: " + tag.barcode + " [Match]";
                                    //tagList.Items.Add(tagTextBox);
                                    tagList.Text = tagList.Text + "EPC: " + tagId + "\r\n";
                                    tagList.Text = tagList.Text + "TID: " + tagTID + "\r\n";
                                    tagList.Text = tagList.Text + "UPC: " + tag.barcode + " [Match]\r\n\r\n";
                                }
                                else
                                {
                                    tag.barcodeMismatch = true;

                                    //tagTextBox.Text = "EPC: " + tagId + "\r\n" + "TID: " + tagTID + "\r\n" + "UPC: " + tag.barcode + " [Mismatch]";
                                    //tagList.Items.Add(tagTextBox);

                                    tagList.Text = tagList.Text + "EPC: " + tagId + "\r\n";
                                    tagList.Text = tagList.Text + "TID: " + tagTID + "\r\n";
                                    tagList.Text = tagList.Text + "UPC: " + tag.barcode + " [Mismatch]\r\n\r\n";
                                    tagList.BackColor = Color.Red;
                                }

                                //Maybe have to rework this condition below:  TODO
                                if (tag.tagType == "SGTIN-96".ToLower())
                                {
                                    string desc = ItemData.getDescription(tag.companyPrefix, tag.itemReference);

                                    if (desc != null)
                                    {
                                        tag.itemDesc = desc;
                                    }
                                    else
                                    {
                                        tag.itemDesc = "No Description";
                                    }
                                }
                            }
                            catch (Exception te)
                            {
                                if (te.Message == "Tag Header Value does not exist.")
                                {
                                    tagList.BackColor = Color.Yellow;
                                    //tagTextBox.Text = "EPC: " + tagId + "\r\n" + "TID: " + tagTID + "\r\n" + "UPC: (Unencoded)";
                                    //tagList.Items.Add(tagTextBox);
                                    
                                    tagList.Text = tagList.Text + "EPC: " + tagId + "\r\n";
                                    tagList.Text = tagList.Text + "TID: " + tagTID + "\r\n";
                                    tagList.Text = tagList.Text + "UPC: " + "(Unencoded)\r\n\r\n";
                                }
                                else if (te.Message == "Tag Decoding error.")
                                {
                                    tagList.BackColor = Color.Yellow;
                                    
                                    //tagTextBox.Text = "EPC: " + tagId + "\r\n" + "TID: " + tagTID + "\r\n" + "UPC: (Decoding Error)";
                                    //tagList.Items.Add(tagTextBox);

                                    tagList.Text = tagList.Text + "EPC: " + tagId + "\r\n";
                                    tagList.Text = tagList.Text + "TID: " + tagTID + "\r\n";
                                    tagList.Text = tagList.Text + "UPC: " + "(Decoding Error)\r\n\r\n";
                                }
                                else
                                {
                                    Program.debugFile.WriteLine("Exception: {0}", te.Message);
                                    Program.debugFile.WriteLine("Exception: {0}", te.StackTrace);
                                    Program.debugFile.WriteLine("Tag Id: {0}", tagId);
                                }
                            }

                            if (!cycleItems.ContainsKey(tagId))
                            {
                                tag.cycleCounted = false;
                                MessageBox.Show("Enviromental Exception");
                            }
                            else
                            {
                                tag.cycleCounted = true;
                            }

                            outputObj.tags.Add(tag);
                            Program.debugFile.WriteLine("Tag: {0}", tagId);
                        }
                        catch (ArgumentException ae)
                        {
                            if (ae.Message == "Value does not fall within the expected range.")
                                continue;//Do nothing duplicate tag in dictionary
                        }
                    }
                }
                catch (NullReferenceException nre)
                {
                    //If the scan was null we need to enable the scan trigger again.
                    Program.readerTrigger.Stage2Notify += new Trigger.TriggerEventHandler(RFIDScanEvent);
                }
            }
            catch (Exception e)
            {
                tagList.Text = tagList.Text + e.ToString();
                Program.debugFile.WriteLine("ERROR:  {0}\r\n{1}", e.Message, e.StackTrace);
                Program.debugFile.Flush();
            }
        }

        void BarcodeScanEvent(object sender, TriggerEventArgs EvtArgs)
        {
            if (!Program.MyReader.Info.IsEnabled)
            {
                Program.MyReader.Actions.Enable();
            }

            Program.MyReader.Actions.Read(Program.MyReaderData);
            Program.MyReader.Actions.ToggleSoftTrigger();

            while (Program.MyReaderData.IsPending)
            {
                System.Threading.Thread.Sleep(10);
            }

            if (Program.MyReaderData.Text != null && Program.MyReaderData.Text != "")
            {
                try
                {
                    Program.MyReader.Actions.Disable();
                }
                catch (Exception e)
                {
                    Program.debugFile.WriteLine("Barcode Disable: {0}", e.Message);
                    Program.debugFile.Flush();
                }

                barcode = Program.MyReaderData.Text.ToString();
                outputObj = new OutputData(barcode);
                outputObj.upcBarcodeEncoding = Program.MyReaderData.Type.ToString();

                barcodeLabel.Text = barcode + "(" + Program.MyReaderData.Type + ")";
                
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(BarcodeScanEvent);
                Program.readerTrigger.Stage2Notify += new Trigger.TriggerEventHandler(RFIDScanEvent);

                Program.MyReader.Actions.Flush();
            }
            else
            {
                //should beep or something if we don't get a good barcode scan...
            }
        }

        private void clearScreenButton(object sender, EventArgs e)
        {
            //Clear the state for another scan...

            barcodeLabel.Text = "Scan Barcode";
            tagList.Text = "";
            tagList.BackColor = Color.Empty;

            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(BarcodeScanEvent);
            }
            catch (Exception e1)
            {
                //Program.debugFile.WriteLine("EventHandler: {0}\r\n{1}", e1.Message, e1.StackTrace);
            }

            try
            {
             Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(RFIDScanEvent);
            }
            catch (Exception e2)
            {
                //Program.debugFile.WriteLine("EventHandler: {0}\r\n{1}", e2.Message, e2.StackTrace);
            }

            Program.readerTrigger.Stage2Notify += new Trigger.TriggerEventHandler(BarcodeScanEvent);
            this.button3.Text = "TIDs";
            this.button3.Click -= new System.EventHandler(this.saveButton);
            this.button3.Click += new System.EventHandler(this.tidButton);
            outputObj = null;
        }

        private void saveButton(object sender, EventArgs e)
        {
            writeDataObj();
            clearScreenButton(sender, e);
        }

        private void tidButton(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < outputObj.tags.ToArray().Length; i++)
                {
                    //Need to select TIDs for tag(s) being saved

                    TIDForm tidForm = new TIDForm(i, outputObj.tags.ToArray()[i], tidOptions);
                    tidForm.Show();
                }

                this.button3.Text = "Save";
                this.button3.Click -= new System.EventHandler(this.tidButton);
                this.button3.Click += new System.EventHandler(this.saveButton);
            }
            catch (NullReferenceException nre)
            {
            }

        }

        public static void writeDataObj()
        {
            string output = outputObj.timeStamp + "," + outputObj.upcBarcodeScan + "," + outputObj.upcBarcodeEncoding;

            foreach (RFIDTag t in outputObj.tags)
            {
                output += "," + t.epcUri + "," + t.TID + "(" + t.tidModel + ")";

                if (t.barcodeMismatch)
                    output += "," + "Mismatch";
                else
                    output += "," + "Match";

                if (t.cycleCounted)
                    output += "," + "()";
                else
                    output += "," + "(E)";

                output += "," + t.hexValue + "," + t.barcode + "," + t.itemDesc;
            }

            Program.dataFile.WriteLine(output);
            Program.dataFile.Flush();
            Program.debugFile.WriteLine("Data is written out!");
        }

        private void returnButton(object sender, EventArgs e)
        {
            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(BarcodeScanEvent);
            }
            catch (Exception e1)
            {
                //Program.debugFile.WriteLine("EventHandler: {0}\r\n{1}", e1.Message, e1.StackTrace);
            }

            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(RFIDScanEvent);
            }
            catch (Exception e2)
            {
                //Program.debugFile.WriteLine("EventHandler: {0}\r\n{1}", e2.Message, e2.StackTrace);
            }

            this.Dispose();
            this.Hide();
        }

        private void exceptionButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ExceptionButtonStuff");
        }
    }
}