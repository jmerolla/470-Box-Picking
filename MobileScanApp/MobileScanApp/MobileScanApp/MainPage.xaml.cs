using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using PCLStorage;
using CsvHelper;
using System.IO;
using ZXing;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace MobileScanApp
{

    /*
     * @author:     Jess Merolla
     * @date:       9/19/2020
     * @summary:
     *
     * This class initializes the start-up page.
     * Contains a single button to begin scanning in the initial
     * order.
     * 
     *  
     *
     *
     *
     */
    public partial class MainPage : ContentPage
    {
        //uses PCLStorage to access cross platform filesystems
        // IFolder folder = PCLStorage.FileSystem.Current.LocalStorage;

        String csvdata;     //make the csv info globally accessible
        List<String> ItemsList = new List<String>();

        CSVHandler orderItemParser = new CSVHandler();    //used to parse the csv order sheets
        string[,] orderArray;   //the csv successfully parsed 
        public List<OrderItem> OrderItems { get; set; } //used to pass the list of items
        public MainPage()
        {
            InitializeComponent();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            // IFolder folder = PCLStorage.FileSystem.Current.LocalStorage;


        }

        /* @author Jess Merolla
         * 
         * Navigates over to the ScanPage (for Scanning Barcodes)
         * 
         * !!!!!!!!!!!!TO-DO Remove this option from the main page on startup
         * 
         */

        /*
         * @author: Jess Merolla
         * @date: 9/25/2020
         * @summary:
         * 
         * Calls a file picker and allows the user to pick a file that will be used for parsing
         * on a button click
         *
         *@param: object sender, EventArgs e
         *
         *
         * !!!!!!!!!!!TO-DO check file type restrictions, do something with the file
         * 
         * 
         * Graham added a call to ReadInCSV at the end of this method 9/27/2020 
         * 
         */
        private async void PickFileButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Specifies csv file type for each platform
                string fileType = null;
                if (Device.RuntimePlatform == Device.Android)
                {
                    fileType = "csv";
                }
                if (Device.RuntimePlatform == Device.UWP)
                {
                  
                    fileType = ".csv";
                }

                //Opens file picker
                FileData filedata = await CrossFilePicker.Current.PickFile();

                //Loop file picker until a .csv is selected
                //skips if picking operation is cancelled
                while (filedata!= null && filedata.FileName.Contains(fileType)!= true){
                    lbl.Text = "File Type must be .csv";
                    filedata = await CrossFilePicker.Current.PickFile();
                }


                //Prints all values of CSV to console
                if(filedata != null)
				{
                    lbl.Text = filedata.FileName;
                    var filePath = filedata.FilePath;
                    if (filedata != null)
                    {
                        lbl.Text = filedata.FileName;
                        csvdata = ReadInCSV(filedata);
                        System.Diagnostics.Debug.Write(csvdata);
                        lbl.Text = csvdata;

                    }
                }
            }
            catch(Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
        /*
        * @author Graham, Jess
        * 9/27/2020
        * Reads in a CSV using a given path and gives a full list of values in a List<String>
        * TODO: -pass parse array info into a list of OrderItems and pass those to OrderListView.xaml.cs
        *       -DONE 10/15 -make the parsing stuff into its own class that gets called in the read CSV method
        *       -DONE 10/10 - Jess figure out how to minimize the blank spaces at the end of the array
        * 
        * 
        */
        public String ReadInCSV(FileData filedata)
        {

            StreamReader reader = new StreamReader(filedata.GetStream());
            string orderText = reader.ReadToEnd();

           orderText =  orderItemParser.getOrderItemInfo(orderText);

            ItemsList = orderText.Split(',').ToList();
            int ORDER_COLUMNS = 12;

            orderArray = orderItemParser.parseOrderItemsIntoArray(ItemsList, ORDER_COLUMNS);

            OrderItems = orderItemParser.arrayToOrderItemList(orderArray);
            
            return orderText;
        }

        /// @author Jessica Merolla
        /// @date 9/29/2020
        /// 
        /// !!!!!!!!!!TODO pass CSV into list view, toggle visibility in xml if csv file has not been picked
        /// 
        /// <summary>
        /// Passes the string of csv data into the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmOrderButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrderListView(ItemsList));

        }
    }
}
