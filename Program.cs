using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Symbol.ResourceCoordination;
using Symbol.RFID3;
using Symbol.Barcode;
using System.Collections;

namespace Handheld
{
    class Program
    {
        // Create new reader, first available reader will be used.
        public static Reader MyReader = null;
        public static ReaderData MyReaderData = null;
        public static Trigger readerTrigger = null;

        public static RFIDReader readerRFID = null;

        public static TagStorageSettings tagStorageSettings = null;
        public static string errorMessages = null;
        
        public static System.IO.TextWriter debugFile = null;
        public static System.IO.TextWriter dataFile = null;
        public static ItemData items = null;

        public static Symbol.RFID3.SESSION currentSession;

        [MTAThread]
        static void Main()
        {
            debugFile = new System.IO.StreamWriter("\\Program Files\\handheld\\output.txt");

            dataFile = new System.IO.StreamWriter("\\Program Files\\handheld\\auditData.txt", true);

            try
            {
                items = new ItemData("\\Program Files\\handheld\\Items.csv");
            }
            catch (Exception e)
            {
                debugFile.WriteLine("ItemData Error: {0}\r\n", e.Message);
                debugFile.Flush();
            }

            try
            {
                Program.connectRFIDReader();

                //Barcode Reader Scanner setup


                MyReaderData = new Symbol.Barcode.ReaderData(Symbol.Barcode.ReaderDataTypes.Text, Symbol.Barcode.ReaderDataLengths.MaximumLabel);
                MyReader = new Symbol.Barcode.Reader();

                MyReader.Actions.Enable();

                //MyReader.Decoders.DisableAll();
                //MyReader.Decoders.SetEnabled(DecoderTypes.UPCA, true);
                //MyReader.Decoders.SetEnabled(DecoderTypes.EAN13, true);
                //MyReader.Decoders.SetEnabled(DecoderTypes.EAN8, true);
                //MyReader.Decoders.SetEnabled(DecoderTypes.CODE128, true);

                //Trigger Setup

                //The MC3090 has two triggers: (1)the Yellow keyboard button and (2)the finger trigger
                readerTrigger = new Trigger(TriggerDevice.AvailableTriggers[TriggerDevice.AvailableTriggers.Length - 1]);

                //Add an Event Handler for Stage2Notify
                //readerTrigger.Stage2Notify += new Symbol.ResourceCoordination.Trigger.TriggerEventHandler(MyTrigger_Stage2Notify);
            }
            catch (Exception e)
            {
                debugFile.WriteLine("Program ERROR: {0}\r\n{1}", e.Message, e.StackTrace);
                debugFile.WriteLine(e.InnerException);
                debugFile.Flush();
            }

            debugFile.WriteLine("Beginning the application run...");
            debugFile.Flush();

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                debugFile.WriteLine("ERROR: {0}\r\n{1}", e.Message, e.StackTrace);
                debugFile.WriteLine(e.InnerException);
                debugFile.Flush();
            }
            finally
            {
                Program.MyReader.Dispose();
                Program.readerRFID.Dispose();
                Program.MyReaderData.Dispose();
                Program.readerTrigger.Dispose();
                debugFile.Flush();
                debugFile.Close();
                dataFile.WriteLine("EOF");
                dataFile.Flush();
                dataFile.Close();
            }
        }

        public static void connectRFIDReader()
        {
            //RFID
            try
            {
                readerRFID = new RFIDReader("127.0.0.1", 0, 0);
                readerRFID.Connect();
            }
            catch (Exception e)
            {
                Program.debugFile.WriteLine("READER CONNECT ERROR: {0}\r\n{1}", e.Message, e.StackTrace);
                Program.debugFile.Flush();
                
            }

            Program.debugFile.WriteLine("Reader Connected: {0}", readerRFID.IsConnected);
            Program.debugFile.Flush();

            //Tag Storage
            tagStorageSettings = new TagStorageSettings(6144,  // buffer size: 6k
                                                             64,  // memory bank size: still use the default value
                                                             12); // supported tag id: still use the default value

            readerRFID.Config.SetTagStorageSettings(tagStorageSettings);

            setAntennaPower(5);
            setAntennaSession(SESSION.SESSION_S1);

            try
            {
                // Read Operation Sequencing:
                // Motorola's API leaves the tag's communication handle open
                // therefore you can get the TID and the EPC with on sequence
                // call.

                TagAccess.Sequence.Operation op = new TagAccess.Sequence.Operation();
                op.AccessOperationCode = ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ;
                op.ReadAccessParams.MemoryBank = MEMORY_BANK.MEMORY_BANK_TID;
                op.ReadAccessParams.ByteCount = 0;
                op.ReadAccessParams.ByteOffset = 0;
                op.ReadAccessParams.AccessPassword = 0;

                Program.readerRFID.Actions.TagAccess.OperationSequence.Add(op);
            }
            catch (Exception e)
            {
                Program.debugFile.WriteLine("SEQUENCE ERR: {0}", e.Message);
                Program.debugFile.Flush();
            }

        }

        public static void setAntennaSession(Symbol.RFID3.SESSION session)
        {
            currentSession = session;

            ushort[] antID = readerRFID.Config.Antennas.AvailableAntennas;

            foreach (ushort antIndex in antID)
            {
                // set the singulation parameter
                Antennas.SingulationControl singularControl = readerRFID.Config.Antennas[antIndex].GetSingulationControl();

                singularControl.Session = session;
                singularControl.TagPopulation = 100;

                singularControl.Action.PerformStateAwareSingulationAction = false;
                //singularControl.Action.InventoryState = INVENTORY_STATE.INVENTORY_STATE_A;
                
                //singularControl.Action.SLFlag = SL_FLAG.SL_FLAG_DEASSERTED;

                readerRFID.Config.Antennas[antIndex].SetSingulationControl(singularControl);
                debugFile.WriteLine("Session set to {0}", readerRFID.Config.Antennas[antIndex].GetSingulationControl().Session.ToString());
                Program.debugFile.Flush();
            }
        }

        public static void setAntennaPower(int powerLevel)
        {
            ushort[] antID = null;

            try
            {

                antID = readerRFID.Config.Antennas.AvailableAntennas;

            }
            catch (Exception e)
            {
                Program.debugFile.WriteLine(e.Message);
                return;
            }

            Antennas.Config antConfig = null;

            foreach (ushort antIndex in antID)
            {
                antConfig = Program.readerRFID.Config.Antennas[antIndex].GetConfig();
                antConfig.TransmitPowerIndex = (ushort) powerLevel;
                readerRFID.Config.Antennas[antIndex].SetConfig(antConfig);
                Program.debugFile.WriteLine("Antenna[{0}] Power: {1}", antIndex, readerRFID.Config.Antennas[antIndex].GetConfig().TransmitPowerIndex.ToString());
                Program.debugFile.Flush();
            }
        }

        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
