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
    public partial class RFIDSweep : Form
    {
        public static Dictionary<string, string> epcBarcode = null;

        public RFIDSweep()
        {

            Program.debugFile.WriteLine("RFID SWEEP...");
            Program.debugFile.Flush();

            Program.debugFile.WriteLine("setting power...");
            Program.debugFile.Flush();
            Program.setAntennaPower(30);

            epcBarcode = new Dictionary<string, string>();

            this.MinimizeBox = false;

            InitializeComponent();

            foreach (Symbol.RFID3.SESSION session in new[] { Symbol.RFID3.SESSION.SESSION_S0, Symbol.RFID3.SESSION.SESSION_S1, Symbol.RFID3.SESSION.SESSION_S2, Symbol.RFID3.SESSION.SESSION_S3 })
            {
                int value = comboBox1.Items.Add(session);
                if(Program.currentSession.Equals(session))
                {
                    comboBox1.SelectedIndex = value;
                }
            }

            Symbol.RFID3.Antennas ant = Program.readerRFID.Config.Antennas;
            
            Program.debugFile.WriteLine("setting trigger pull...");
            Program.debugFile.Flush();

            Program.readerTrigger.Stage2Notify += new Trigger.TriggerEventHandler(this.RFIDScanEvent);
            Program.readerRFID.Events.ReadNotify += new Events.ReadNotifyHandler(Events_ReadNotify);
        }

        void RFIDScanEvent(object sender, TriggerEventArgs EvtArgs)
        {
            //Symbol.RFID3.TagData[] remainingTags = null;
            
            //RFIDTag tag = null;

            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(this.RFIDScanEvent);
                Program.readerRFID.Actions.TagAccess.OperationSequence.PerformSequence();

                while (Program.readerTrigger.State != TriggerState.RELEASED)
                {
                    try
                    {
                        try
                        {
                            
                            System.Threading.Thread.Sleep(20);

                            //Program.readerRFID.Actions.TagAccess.OperationSequence.StopSequence();
                            //remainingTags = Program.readerRFID.Actions.GetReadTags(100);
                            //Program.debugFile.WriteLine("Count of {1}", remainingTags.Length);
                        }
                        catch (Exception ee)
                        {
                            Program.dataFile.WriteLine("Error: {0}", ee.Message);
                            continue;
                        }

                        /*for (int i = 0; i < remainingTags.Length; i++)
                        {
                            try
                            {
                                string tagId = remainingTags[i].TagID;
                                try
                                {
                                    tag = new RFIDTag(tagId);
                                    epcBarcode.Add(tagId, tag.barcode);
                                    tagList.Text = tagList.Text + tagId + "(" + tag.barcode + ")\r\n";
                                }
                                catch (Exception te)
                                {
                                    if (te.Message == "Tag Header Value does not exist.")
                                    {
                                        epcBarcode.Add(tagId, "Unencoded");
                                        tagList.Text = tagList.Text + tagId + "(" + tag.barcode + ")\r\n";
                                    }
                                }
                            }
                            catch (ArgumentException ae)
                            {
                                if (ae.Message == "Value does not fall within the expected range.")
                                    continue;//Do nothing duplicate tag in dictionary
                            }
                        }
                        this.counter.Text = epcBarcode.Count.ToString();
                        this.Refresh();
                        Program.readerRFID.Actions.TagAccess.OperationSequence.PerformSequence();
                         */
                    }
                    catch (Exception e)
                    {
                        Program.debugFile.WriteLine("Error: {0}",e.Message);
                    }
                }

                Program.readerRFID.Actions.TagAccess.OperationSequence.StopSequence();
            }
            catch (Exception e)
            {
                tagList.Text = tagList.Text + e.ToString();
                Program.debugFile.WriteLine("ERROR:  {0}\r\n{1}", e.Message, e.StackTrace);
            }
        }


        // Read Notify handler
        public void Events_ReadNotify(object sender, Events.ReadEventArgs e)
        {
            try
            {
                Program.debugFile.WriteLine("Tag Event Read Notify: {0}", e.ReadEventData.TagData.TagID);
                Program.debugFile.Flush();
                this.tagList.Text = this.tagList.Text + "\r\n" + ".";
            }
            catch (Exception err)
            {
                Program.debugFile.WriteLine("ERROR: {0}\r\n{1}", err.Message, err.StackTrace);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Program.readerTrigger.Stage2Notify -= new Trigger.TriggerEventHandler(RFIDScanEvent);
            }
            catch (Exception ex)
            {

            }

            Program.debugFile.WriteLine("RETURN");
            Program.debugFile.Flush();
            this.Dispose();
            this.Hide();
        }

        private void sessionSetButton(object sender, EventArgs e)
        {
            Program.setAntennaSession((Symbol.RFID3.SESSION)comboBox1.SelectedItem);
        }
    }
}