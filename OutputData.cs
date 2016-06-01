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
    public class OutputData
    {
        public string timeStamp { get; set; }
        public string upcBarcodeScan { get; set; }
        public string upcBarcodeEncoding { get; set; }
        public string barcodeDesc { get; set; }
        public List<RFIDTag> tags;// { get; set; }

        public OutputData()
        {
        }

        public OutputData(String barcode)
        {
            timeStamp = DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss");

            upcBarcodeScan = barcode;

            try
            {
                barcodeDesc = Program.items.get()[upcBarcodeScan];
            }
            catch (NullReferenceException nre)
            {
                barcodeDesc = "No Description";
            }

            tags = new List<RFIDTag>();
        }
    }
}