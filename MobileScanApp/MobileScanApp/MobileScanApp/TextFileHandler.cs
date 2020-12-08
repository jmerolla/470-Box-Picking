using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using ZXing.QrCode.Internal;

namespace MobileScanApp
{
    /// <summary>
    /// @author Jess Merolla
    /// @date 10/15/2020
    /// 
    /// Methods for extracting CSV data and turning that data into OrderItems
    /// </summary>
   class TextFileHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderText"></param>
        /// <returns></returns>
        public String getOrderHeader(String orderText)
        {
            string pattern = @"Ln\sItem"; //find where item specific text begins
            int matchInt = 1;

            foreach (Match match in Regex.Matches(orderText, pattern))
            {
                matchInt = match.Index;

            }
            string headerText = orderText.Remove(matchInt);
            headerText = headerText.Trim(' ');

            return headerText;
        }


        /// <summary>
        /// @author Jess Merolla
        /// @date 10/15/2020
        /// Takes a string holding csv data and returns a string
        /// with the information unrelated to the items ordered removed
        /// </summary>
        /// <param name="orderText">The string holding all CSV data</param>
        /// <returns>The trimmed string of CSV data</returns>
        public String removeEndOfOrder(String orderText)
        {
      
            //REMOVED AS THE ORDER HEADER WILL LIKELY NEED TO BE EXTRACTED AS WELL
           // string pattern = @"Ln";
            //string pattern = @"Ln\s"; For use with csvs made with current method (broken)
            int matchInt = 1;

           // foreach (Match match in Regex.Matches(orderText, pattern))
            //    matchInt = match.Index;


           // orderText = orderText.Substring(matchInt);

            //pattern = @"--";
            string pattern = @"Base\sOrder";
            foreach (Match match in Regex.Matches(orderText, pattern))
            {
                matchInt = match.Index;
               
            }
            orderText = orderText.Remove(matchInt);
            orderText = orderText.Trim(' ');

            return orderText;

        }

        /// <summary>
        /// @author Jess Merolla
        /// @date 11/4/2020
        /// 
        /// Extracts the header information and then removes it from the string of order text
        /// 
        /// TODO Needs to extract the header and do something with it - waiting on Bill
        /// </summary>
        /// <param name="orderText"></param>
        /// <returns></returns>
        public String removeHeaderInfo(String orderText)
        {
            //TODO take whats needed from the header

            //Remove all unnecessary header data
            string pattern = @"Location\r\n";
            Match m = Regex.Match(orderText, pattern);
            orderText = orderText.Substring(m.Index);

            //get start of specific order information
            pattern = @"\s1\s";
            m = Regex.Match(orderText, pattern);
            orderText = orderText.Substring(m.Index);

            return orderText;
        }

        /*
            /// <summary>
            /// 
            /// Takes a list of Strings to fill and return a 2d array with columns of length ORDER_COLUMNS. 
            /// *NOTE: This preserves the format of the original csv order  sheet the list comes from.*
            /// 
            /// </summary>
            /// <param name="ItemsList">the list of substrings taken from the parsed csv file</param>
            /// <param name="ORDER_COLUMNS">the amount of columns in the original csv file, used to determine
            /// the row and column count of the array</param>
            /// <returns>a 2D array of strings from ItemsList</returns>
            public String[,] parseOrderItemsIntoArray(List<String> ItemsList, int ORDER_COLUMNS)
        {
            int k = 0; //itemsList iterator
            string[,] orderArray = new string[ItemsList.Count / ORDER_COLUMNS, ORDER_COLUMNS];
            for (int i = 0; i < orderArray.GetLength(0); i++)
            {
                for (int j = 0; j < orderArray.GetLength(1); j++)
                {
                    if (k < ItemsList.Count)
                    {
                        //if you don't fill array until ItemsList has a valid item
                        //*IF UNCOMMENTED, will not preserve original layout of the item sheet*
                        // while (String.IsNullOrEmpty(ItemsList[k])) //!= null && ItemsList[k] != "")
                        //{
                        //  k++;
                        //}
                        orderArray[i, j] = ItemsList[k];
                        k++;
                    }
                }
            }
            return orderArray;
        }

        
        public List<OrderItem> arrayToOrderItemList(string[,] orderArray)
        {
            List<OrderItem> OrderItems = new List<OrderItem>();
            Regex rx = new Regex(@"\r\n[0-9]+",
                       RegexOptions.Compiled | RegexOptions.Singleline);
            for (int i = 0; i < orderArray.GetLength(0); i++)
            {
                if(i%2 != 0) {  //odd row
                    if (rx.IsMatch(orderArray[i, 0])){  //make sure there is an item in this row

                        try
                        {
                            OrderItems.Add(new OrderItem
                            {
                                IsPacked = false,
                                Name = orderArray[i + 1, 1],
                                Location = orderArray[i, 11],
                                BarcodeID = orderArray[i, 1],
                                //first three were originally decimal
                                PalletQty = decimal.Parse(orderArray[i, 8]),
                                CartonQty = decimal.Parse(orderArray[i, 9]),
                                QtyOrdered = decimal.Parse(orderArray[i, 3]),
                                QtyOpen = decimal.Parse(orderArray[i, 4])
                            });
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        
                    }
                }
            }

            return OrderItems;
        }
        */


        /// <summary>
        /// @Author Jess Merolla
        /// @date 11/5/2020
        /// 
        /// Take the String list of parsed order sheet info and convert that into a list of OrderItems
        /// </summary>
        /// <param name="ItemsList"></param>
        /// <returns></returns>
        public List<OrderItem> parseOrderItemsFromList(List<string> ItemsList)
        {
            List<OrderItem> OrderItems = new List<OrderItem>();

            //Regex used to identify order item data
            Regex ln = new Regex(@"[0-9]{1,2}", RegexOptions.Compiled | RegexOptions.Singleline);
            //item number originally just {14}
            Regex ItemNumber = new Regex(@"[0-9]{10,}", RegexOptions.Compiled | RegexOptions.Singleline);
            Regex ItemName = new Regex(@"[a-zA-Z]{2,}", RegexOptions.Compiled | RegexOptions.Singleline);
            Regex Location = new Regex(@"[a-zA-Z]{1}[0-9]+", RegexOptions.Compiled | RegexOptions.Singleline);

            //Used to hold location
            String locationString;
            for (int i=0; i< ItemsList.Count; i++)
            {
                if (i + 1 < ItemsList.Count)    //prevent null error
                {
                    if (ln.IsMatch(ItemsList[i]) && ItemNumber.IsMatch(ItemsList[i+1]))
                    {
                      
                        locationString = "";
                        int j = i+10;   //start at first location value

                        //loop through and find all possible locations,
                        //stopping at the next ItemName field
                        while (ItemName.IsMatch(ItemsList[j])!= true)
                        {
                            if (Location.IsMatch(ItemsList[j]))
                            {
                                ItemsList[j] = ItemsList[j].Remove(3);
                                locationString+=(ItemsList[j] + " (" + ItemsList[j-1] + ") \r\n");
                            }

                            j++;
                        }
                
                         try
                         {
                             OrderItems.Add(new OrderItem
                             {
                                 IsPacked = false,
                                 Name = ItemsList[j] + " " + ItemsList[j+1],
                                 LocationQOH = locationString, 
                                 BarcodeID = ItemsList[i+1],

                                 PalletQty = decimal.Parse(ItemsList[i+7]),
                                 CartonQty = decimal.Parse(ItemsList[i+8]),
                                 QtyOrdered = decimal.Parse(ItemsList[i+3]),   
                                 QtyOpen = decimal.Parse(ItemsList[i+4]),

                                 UM = ItemsList[i+2],
                                 ExtPrice = ItemsList[i+5],
                                 DueDate = ItemsList[i+6]
                             });
                         }
                         catch (FormatException e)
                         {
                             Console.WriteLine(e.Message);
                         }
                        
                    }
                }
            }
            return OrderItems;
        }
    }
}
