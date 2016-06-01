using System;

using System.Collections.Generic;
using System.Text;

using Symbol.ResourceCoordination;
using Symbol.Barcode;
using Symbol.StandardForms;
using Symbol;

namespace Handheld
{
    public class BarcodeScanner
    {
        protected static BarcodeScanner _instance;
        protected static readonly object scannerLock = new object();

        public string BarcodeScanData;

        public delegate void BarcodeScanEventHandler(object sender, string barcode);
        public event BarcodeScanEventHandler BarcodeScanEvent;

        protected Reader _reader;
        protected ReaderData _readerData;
        private System.EventHandler myReadNotifyHandler = null;

        //private Trigger _readerTrigger;
        //private Trigger.TriggerEventHandler myStage2NotifyHandler = null;


        public BarcodeScanner()
        {
        }

        ~BarcodeScanner()
        {
            if (_reader != null) { _reader.Dispose(); }
            _reader = null;
        }

        protected virtual void BarcodeScan(string barcode)
        {
            if (barcode != null)
                BarcodeScanEvent(this, barcode);
        }

        public void StartScanning()
        {
            if (_reader != null) _reader.Dispose();

            //add bar code scanner object and start thread so we can capture bar code events.
            _reader =
                new Symbol.Barcode.Reader(
                    Symbol.StandardForms.SelectDevice.Select(
                    Device.Title, Device.AvailableDevices));

            _readerData = new ReaderData(ReaderDataTypes.Text, ReaderDataLengths.MaximumLabel);
            _reader.Actions.Enable();
            activateLaser();

            this.myReadNotifyHandler = new EventHandler(myReader_ReadNotify);
            _reader.ReadNotify += new EventHandler(myReadNotifyHandler);

        }

        public void StopScanning()
        {
            deactivateLaser();
            dispose();
        }

        void dispose()
        {
            if (_reader != null)
            {
                _reader.Actions.Flush();
                _reader.Actions.Disable();
                _reader.Dispose();
            }
            if (_readerData != null)
            {
                _readerData.Dispose();
            }
        }

        private void myReader_ReadNotify(object Sender, EventArgs e)
        {
            ReaderData scanData = this._reader.GetNextReaderData();
            if (scanData != null && scanData.Result == Symbol.Results.SUCCESS && scanData.Text != null)
            {
                BarcodeScanData = scanData.Text;
                BarcodeScan(BarcodeScanData);
            }
        }

        void activateLaser()
        {
            _reader.Actions.Read(_readerData);
            _reader.Info.SoftTrigger = true;
        }

        void deactivateLaser()
        {
            _reader.Info.SoftTrigger = false;
            _reader.Actions.Flush();
        }
    }

}