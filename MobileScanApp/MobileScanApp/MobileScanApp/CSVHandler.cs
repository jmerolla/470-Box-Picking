using System;
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
   class CSVHandler
    {
        /// <summary>
        /// Takes a string holding csv data and returns a string
        /// with the information unrelated to the items ordered removed
        /// </summary>
        /// <param name="orderText">The string holding all CSV data</param>
        /// <returns>The trimmed string of CSV data</returns>
        public String getOrderItemInfo(String orderText)
        {

            string pattern = @"Ln,";
            //string pattern = @"Ln\s"; For use with csvs made with current method (broken)
            int matchInt = 1;

            foreach (Match match in Regex.Matches(orderText, pattern))
                matchInt = match.Index;


            orderText = orderText.Substring(matchInt);

            pattern = @"Base\sOrder";
            foreach (Match match in Regex.Matches(orderText, pattern))
                matchInt = match.Index;

            orderText = orderText.Remove(matchInt);
            orderText = orderText.Trim(',', ' ');

            return orderText;

        }

        /// <summary>
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
                                Name = orderArray[i + 1, 1],
                                Location = orderArray[i, 11],
                                BarcodeID = orderArray[i, 1],

                                PalletQty = Int32.Parse(orderArray[i, 8]),
                                CartonQty = Int32.Parse(orderArray[i, 9]),
                                QtyOrdered = Int32.Parse(orderArray[i, 3]),
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
    }
}
