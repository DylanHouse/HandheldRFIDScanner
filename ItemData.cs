using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace Handheld
{
    public class ItemData
    {
        public static Dictionary<string, string> itemData;

        public Dictionary<string, string> get()
        {
            return itemData;
        }


        public ItemData(string fileName)
        {
            itemData = new Dictionary<string, string>();
            
            System.IO.StreamReader itemFile = new System.IO.StreamReader(fileName);
            string line;

            line = itemFile.ReadLine();
            Program.debugFile.WriteLine("First Line: {0}", line);
            int number = 0;

            while ( (line = itemFile.ReadLine()) != null)
            {
                string[] splitLine = line.Split(',');

                string upc = Regex.Replace(splitLine[0], @"^0+", "");

                object[] array = new string[splitLine.Length - 1];
                Array.Copy(splitLine, 1, array, 0, array.Length);
                
                try
                {
                    string description = "";

                    try
                    {
                        description = array[3].ToString() + " " + array[2].ToString() + "(" + array[4].ToString() + ")";
                    }
                    catch (IndexOutOfRangeException iore)
                    {
                        description = "ERROR IN FILE DESCRIPTION";
                    }

                    if (!itemData.ContainsKey(upc))
                    {
                        itemData.Add(upc, description);
                    }
                }
                catch (Exception e)
                {
                    if (number == 0)
                        Program.debugFile.WriteLine("Line info: {0}", line);

                    Program.debugFile.WriteLine("ERROR[{1}]: {0}", e.Message, ++number);
                    Program.debugFile.WriteLine(e.StackTrace);
                }
            }
        }

        public static string getDescription(string coPrefix, string itemRef)
        {
            coPrefix = Regex.Replace(coPrefix, @"^0+", "");
            itemRef = Regex.Replace(itemRef, @"^0+", "");

            bool coPrefixSearch = true;

            foreach(System.Collections.Generic.KeyValuePair<string,string> pair in ItemData.itemData)
            {
                try
                {
                    //Barcode = key
                    string key = pair.Key;
                    string aMatch = Regex.Replace(key, @"^0+", "").Substring(0, coPrefix.Length);

                    if (aMatch.ToString() == coPrefix.ToString())
                    {
                        aMatch = pair.Key.Substring(coPrefix.Length);
                        aMatch = Regex.Replace(aMatch, @"^0+", "");
                        aMatch = aMatch.Substring(0, itemRef.Length); //Remove check digit if there is one...
                        
                        if (aMatch == itemRef)
                        {
                            return pair.Value;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException aore)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Program.debugFile.WriteLine("Item Match Error: {0}", e.Message);
                    break;
                }
            }

            return null;
        }
    }
}
