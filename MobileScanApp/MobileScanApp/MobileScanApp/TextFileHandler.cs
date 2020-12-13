using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MobileScanApp
{
    /// <summary>
    /// @author Jess Merolla
    /// @date 10/15/2020
    /// 
    /// Methods for extracting order data and turning that data into OrderItems
    /// </summary>
    class TextFileHandler
    {
        /// <summary>
        /// @author Jess Merolla
        /// @date 11/4/2020
        /// Parses out the order header 
        /// </summary>
        /// <param name="orderText"></param>
        /// <returns>The header's text </returns>
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
        /// Parses out the excess text found after all OrderItems are located
        /// </summary>
        /// <param name="orderText">The string holding all order data</param>
        /// <returns>The trimmed string of order data</returns>
        public String removeEndOfOrder(String orderText)
        {
            int matchInt = 1;

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
        /// </summary>
        /// <param name="orderText">The original string including all order data along with the header</param>
        /// <returns name="orderText">The string of order data with the header removed</returns>
        public String removeHeaderInfo(String orderText)
        {
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

        /// <summary>
        /// @Author Jess Merolla
        /// @date 11/5/2020
        /// 
        /// Take the String list of parsed order sheet info and convert that into a list of OrderItems
        /// 
        /// Note: regex could be subject to change based on further information about the order sheet format
        /// 
        /// TO-DO:
        ///        -DONE 12/8 -Add comments to explain regex
        ///        -Create constants for the different locations referenced in ItemsList
        /// </summary>
        /// <param name="ItemsList"></param>
        /// <returns name="OrderItems">The list of order items</returns>
        public List<OrderItem> parseOrderItemsFromList(List<string> ItemsList)
        {
            List<OrderItem> OrderItems = new List<OrderItem>();

            //Regex used to identify order item data

            //ln - "line", Example "1" as in the first item in the order
            //Looks for a number from 0-99, assumed an item sheet will not have 100+ items
            Regex ln = new Regex(@"[0-9]{1,2}", RegexOptions.Compiled | RegexOptions.Singleline);
            //ItemNumber - represents the barcode,  originally just {14} characters long,
            //but changed to 10+ for testing purposes
            Regex ItemNumber = new Regex(@"[0-9]{10,}", RegexOptions.Compiled | RegexOptions.Singleline);
            //ItemName - String name of the item, looks for at least two upper or lowercase letters
            Regex ItemName = new Regex(@"[a-zA-Z]{2,}", RegexOptions.Compiled | RegexOptions.Singleline);
            //Location - the physical place where the items are held, Example "s01" 
            //looks for one upper or lowercase letter, followed by 1 or more numbers
            Regex Location = new Regex(@"[a-zA-Z]{1}[0-9]+", RegexOptions.Compiled | RegexOptions.Singleline);

            //Used to hold location and quantity of an item
            String locationAndQOHString;
            for (int i=0; i< ItemsList.Count; i++)
            {
                if (i + 1 < ItemsList.Count)    //prevent null error
                {
                    if (ln.IsMatch(ItemsList[i]) && ItemNumber.IsMatch(ItemsList[i+1]))
                    {
                      
                        locationAndQOHString = "";
                        int j = i+10;   //start at first location value

                        //loop through and find all possible locations,
                        //stopping at the next ItemName field
                        while (ItemName.IsMatch(ItemsList[j])!= true)
                        {
                            if (Location.IsMatch(ItemsList[j]))
                            {
                                //removes the line break characters from the location
                                //adds them back in after the quantity is added to the locationAndQOHString
                                ItemsList[j] = ItemsList[j].Remove(3);
                                locationAndQOHString+=(ItemsList[j] + " (" + ItemsList[j-1] + ") \r\n");
                            }

                            j++;
                        }
                
                         try
                         {
                             OrderItems.Add(new OrderItem
                             {
                                 IsPacked = false,
                                 Name = ItemsList[j] + " " + ItemsList[j+1],
                                 LocationQOH = locationAndQOHString, 
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
