using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Handheld
{
    public class RFIDTag
    {
        public string epcUri { get; set; }
        public string barcode { get; set; }
        public string hexValue { get; set; }
        public string binaryValue { get; set; }
        public string tagType { get; set; }
        public string TID { get; set; }
        public string tidModel { get; set; }

        public int partitionValue { get; set; }
        public string companyPrefix { get; set; }
        public string itemReference { get; set; }
        public string serialNumber { get; set; }

        public string serviceReference { get; set; }

        //Exceptions
        public bool barcodeMismatch { get; set; }
        public bool epcEncoded { get; set; }
        public bool cycleCounted { get; set; }

        public string itemDesc { get; set; }

        private StreamWriter debugFile = null;

        private void setValuesToNull()
        {
            epcUri = "NULL";
            barcode = "NULL";
            hexValue = "NULL";
            tagType = "NULL";
            TID = "NULL";
            itemDesc = "No Description";
            barcodeMismatch = true;
            cycleCounted = false;
            tidModel = "NULL";
        }

        public RFIDTag()
        {
            setValuesToNull();
        }

        public RFIDTag(StreamWriter output)
        {
            setValuesToNull();
            debugFile = output;
            debugFile.AutoFlush = true;
        }

        public RFIDTag(string hex, StreamWriter output) : this(hex)
        {
            debugFile = output;
        }

        public RFIDTag(string hex, string tid) : this(hex)
        {
            if (tid != null)
            {
                TID = tid;
            }
        }

        public RFIDTag(string hex)
        {
            setValuesToNull();

            if (debugFile != null)
            {
                debugFile.WriteLine("RFID({0}) invoked...", hex);
            }

            hexValue = hex.ToLower();

            if (hexValue.Length != 24)
            {
                throw new Exception("Invalid hex value length.");
            }

            foreach (char character in hexValue)
            {
                string binaryChar;

                if (character >= '0' && character <= '9')
                {
                    binaryChar = Convert.ToString(Convert.ToInt32(character.ToString(), 16), 2);

                    //Pad to byte length
                    while (binaryChar.Length < 4)
                    {
                        binaryChar = "0" + binaryChar;
                    }

                    binaryValue = binaryValue + binaryChar;

                    continue;
                }
                else if (character >= 'a' && character <= 'f')
                {
                    binaryChar = Convert.ToString(Convert.ToInt32(character.ToString(), 16), 2);

                    while (binaryChar.Length < 4)
                    {
                        binaryChar = "0" + binaryChar;
                    }

                    binaryValue = binaryValue + binaryChar;

                    continue;
                }
                else
                {
                    throw new Exception("Invalid hex string.");
                }
            }

            if (debugFile != null)
            {
                debugFile.WriteLine("Binary Value: {0}", binaryValue);
            }

            // Extract the header value to determine tag type
            setTagType();

        }

        private void gsrn96Decode()
        {
            if (debugFile != null)
            {
                debugFile.WriteLine("Tag Type: {0}", tagType);
            }

            /* Partition | GS1 Company Prefix  | ItemReference
             *  Value    |                     |Indicator/Pad Digit
             * __________|_____________________|___________________
             *    (P)    |  Bits(M) | Digits(L)| Bits (N) | Digits
             * __________|__________|__________|__________|________
             *     0         40         12         18         5
             *     1         37         11         21         6
             *     2         34         10         24         7
             *     3         30         9          28         8
             *     4         27         8          31         9
             *     5         24         7          34         10
             *     6         20         6          38         11  
             * _____________________________________________________
             *     Table 32. GSRN Partition Table EPC TDS 1.5
             */

            // [P][{M,N}] represented below by the 2 dimentional array
            // L = 12 - P
            // Digits = P + 5

            int[][] partitionTable = { 
                                      new int[] { 40, 18 }, 
                                      new int[] { 37, 21 }, 
                                      new int[] { 34, 24 }, 
                                      new int[] { 30, 28 }, 
                                      new int[] { 27, 31 }, 
                                      new int[] { 24, 34 }, 
                                      new int[] { 20, 38 }
                                     };

            partitionValue = Convert.ToInt32(binaryValue.Substring(11, 3), 2);

            if (debugFile != null)
            {
                debugFile.WriteLine("Partition Value: {0}", partitionValue);
            }

            companyPrefix = Convert.ToUInt64(binaryValue.Substring(14, partitionTable[partitionValue][0]), 2).ToString();

            while (companyPrefix.Length < (12 - partitionValue))
            {
                companyPrefix = "0" + companyPrefix;
            }

            if (debugFile != null)
            {
                debugFile.WriteLine("Company Prefix: {0}", companyPrefix);
            }

            serviceReference = Convert.ToUInt64(binaryValue.Substring(14 + partitionTable[partitionValue][0], partitionTable[partitionValue][1]), 2).ToString();

            while (serviceReference.Length < (partitionValue + 5))
            {
                serviceReference = "0" + serviceReference;
            }
            //d18 =(–3(d1 +d3 +d5 +d7 +d9 +d11+d13+d15+d17)–(d2 +d4 +d6 +d8 +d10 +d12+d14+d16))mod10
            
            barcode = companyPrefix + serviceReference;

            Program.debugFile.WriteLine("GSRN Length: {0}\r\nValue: {1}", barcode.Length, barcode);

            barcode = barcode + calculateCheckDigit(barcode);

            if (debugFile != null)
            {
                debugFile.WriteLine("GSRN Barcode: {0}", barcode);
            }

            epcUri = tagType + ":" + companyPrefix + ":" + serviceReference;

            if (debugFile != null)
            {
                debugFile.WriteLine("EPC URI: {0}", epcUri);
            }

        }

        private void sgtin96Decode()
        {

            if (debugFile != null)
            {
                debugFile.WriteLine("Tag Type: {0}", tagType);
            }

            /* Partition | GS1 Company Prefix  | ItemReference
             *  Value    |                     |Indicator/Pad Digit
             * __________|_____________________|___________________
             *    (P)    |  Bits(M) | Digits(L)| Bits (N) | Digits
             * __________|__________|__________|__________|________
             *     0         40         12         4          1
             *     1         37         11         7          2
             *     2         34         10         10         3
             *     3         30         9          14         4
             *     4         27         8          17         5
             *     5         24         7          20         6
             *     6         20         6          24         7   
             * _____________________________________________________
             *     Table 17. SGTIN Partition Table EPC TDS 1.5
             */

            // [P][{M,N}] represented below by the 2 dimentional array
            // L = 12 - P
            // Digits = P + 1

            int[][] partitionTable = { 
                                      new int[] { 40, 4 }, 
                                      new int[] { 37, 7 }, 
                                      new int[] { 34, 10 }, 
                                      new int[] { 30, 14 }, 
                                      new int[] { 27, 17 }, 
                                      new int[] { 24, 20 }, 
                                      new int[] { 20, 24 }
                                     };

            partitionValue = Convert.ToInt32(binaryValue.Substring(11, 3), 2);

            if (debugFile != null)
            {
                debugFile.WriteLine("Partition Value: {0}", partitionValue);
            }

            companyPrefix = Convert.ToUInt64(binaryValue.Substring(14, partitionTable[partitionValue][0]), 2).ToString();

            while (companyPrefix.Length < (12 - partitionValue))
            {
                companyPrefix = "0" + companyPrefix;
            }

            if (debugFile != null)
            {
                debugFile.WriteLine("Company Prefix: {0}", companyPrefix);
            }

            itemReference = Convert.ToUInt64(binaryValue.Substring(14 + partitionTable[partitionValue][0], partitionTable[partitionValue][1]), 2).ToString();

            while (itemReference.Length < (partitionValue + 1))
            {
                itemReference = "0" + itemReference;
            }

            if (debugFile != null)
            {
                debugFile.WriteLine("Item Reference: {0}", itemReference);
            }

            //urn:epc:id:sgtin:d2d3...d(L+1).d1d(L+2)d(L+3)...d13.s1s2...sK
            // Note:  d1 is the first number of the item reference value
            // the barcode of an SGTIN-96 is a GTIN 14 these get encode using the
            // UPCA and UPCB barcode standards

            try
            {
                barcode = itemReference.Substring(0, 1) + companyPrefix + itemReference.Substring(1);
            }
            catch (ArgumentOutOfRangeException aore)
            {
                if (itemReference.Length == 1)
                {
                    barcode = itemReference + companyPrefix + calculateCheckDigit(barcode);
                }
                else
                {
                    throw aore;
                }
            }

            //To calculate the check digit for GTIN 14 (GTIN 8, GTIN 12, & GTIN 13 need to be padded with 0s)
            while (barcode.Length < 13)
            {
                barcode = "0" + barcode;
            }

            barcode = barcode + calculateCheckDigit(barcode);

            if (debugFile != null)
            {
                debugFile.WriteLine("GTIN 14 Barcode: {0}", barcode);
            }

            serialNumber = Convert.ToUInt64(binaryValue.Substring((binaryValue.Length - 38), 38), 2).ToString();

            if (debugFile != null)
            {
                debugFile.WriteLine("Serial Number: {0}", serialNumber);
            }

            epcUri = tagType + ":" + companyPrefix + ":" + itemReference + ":" + serialNumber;

            if (debugFile != null)
            {
                debugFile.WriteLine("EPC URI: {0}", epcUri);
            }
        }

        private void sgtin96Encode()
        {

        }

        public string calculateCheckDigit(string input)
        {
            //Calculate the check digit dn =(–3(d1 + d(1+2) + ... + d(L))–(d2 + d(2+2) + ... + d(L-1))) mod 10
            if (input.Length % 2 != 1)
            {
                throw new Exception("Check Digit Input Length invalid.");
            }

            if (debugFile != null)
            {
                debugFile.WriteLine("Calculating check digit for: {0}", input);
            }

            int[] oddArray = new int[ (input.Length / 2) + 1 ];
            int[] evenArray = new int[ (input.Length/ 2) ];

            int checkDigit = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if ((i + 1) % 2 == 0)//even
                {
                    evenArray[((i - 1) / 2)] = Convert.ToInt32(input.ToCharArray()[i].ToString());

                    if (debugFile != null)
                    {
                        debugFile.WriteLine("\td{1} = {0}", Convert.ToInt32(input.ToCharArray()[i].ToString()), i + 1);
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        oddArray[((i - 1) / 2)] = Convert.ToInt32(input.ToCharArray()[i].ToString());
                    }
                    else
                    {
                        oddArray[0] = Convert.ToInt32(input.ToCharArray()[i].ToString());
                    }

                    if (debugFile != null)
                    {
                        debugFile.WriteLine("\td{1} = {0}", Convert.ToInt32(input.ToCharArray()[i].ToString()), i + 1);
                    }
                }
            }
            foreach (int i in oddArray)
            {
                checkDigit += (-3 * i);
            }

            foreach (int i in evenArray)
            {
                checkDigit -= i;
            }

            checkDigit = (Math.Abs(checkDigit * 10) + checkDigit) % 10;

            if (debugFile != null)
            {
                //Calculate the check digit d14 =( –3 (d1 + d3 + d5 + d7 + d9 + d11 + d13) – (d2 + d4 + d6 + d8 + d10 + d12)) mod 10

                debugFile.Write("\td14 = ( -3 (d1 + d3 + d5 + d7 + d9 + d11 + d13)");
                debugFile.WriteLine(" - (d2 + d4 + d6 + d8 + d10 + d12)) mod 10");

                debugFile.Write("\t{0} = ( -3 ", checkDigit);

                for (int j = 0; j < oddArray.Length; j++)
                {
                    if (j == 0)
                    {
                        debugFile.Write("({0}", oddArray[j]);
                    }
                    else
                    {
                        debugFile.Write(" + {0}", oddArray[j]);
                    }
                }

                debugFile.Write(") - ");

                for (int j = 0; j < evenArray.Length; j++)
                {
                    if (j == 0)
                    {
                        debugFile.Write("({0}", evenArray[j]);
                    }
                    else
                    {
                        debugFile.Write(" + {0}", evenArray[j]);
                    }
                }

                debugFile.WriteLine(")) mod 10");
            }

            return checkDigit.ToString();
        }

        private void setTagType()
        {
            try
            {
                switch (binaryValue.Substring(0, 8))
                {
                    // Table 16. EPC Binary Header Values (EPC TDS v 1.5)
                    case "00101100":
                        this.tagType = "GDTI-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00101101":
                        this.tagType = "GSRN-96".ToLower();
                        this.epcEncoded = true;
                        gsrn96Decode();
                        break;

                    case "00101111":
                        this.tagType = "DoD-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110000":
                        this.tagType = "SGTIN-96".ToLower();
                        this.epcEncoded = true;
                        sgtin96Decode();
                        break;

                    case "00110001":
                        this.tagType = "SSCC-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110010":
                        this.tagType = "SGLN-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110011":
                        this.tagType = "GRAI-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110100":
                        this.tagType = "GIAI-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110101":
                        this.tagType = "GID-96";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110110":
                        this.tagType = "SGTIN-198";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00110111":
                        this.tagType = "GRAI-170";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00111000":
                        this.tagType = "GIAI-202";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00111001":
                        this.tagType = "SGLN-195";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    case "00111010":
                        this.tagType = "GDTI-113";
                        this.epcEncoded = true;
                        throw new Exception("Tag Header Value does not exist.");
                        //break;

                    default:
                        this.tagType = "Unknown";
                        this.epcEncoded = false;
                        throw new Exception("Tag Header Value does not exist.");
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Tag Header Value does not exist.")
                    throw (e);

                Program.debugFile.WriteLine("Error: {0}\r\nStack:{1}", e.Message, e.StackTrace);
                Program.debugFile.Flush();
                throw new Exception("Tag Decoding error.");
            }
        }
    }
}
