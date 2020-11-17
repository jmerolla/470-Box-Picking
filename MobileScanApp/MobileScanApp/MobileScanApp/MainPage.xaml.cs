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
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

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
        //Used to refernece the log file
        string fileName;
        IFile file;
        String csvdata;     //make the csv info globally accessible
        List<String> ItemsList = new List<String>();

        CSVHandler orderItemParser = new CSVHandler();    //used to parse the csv order sheets
        string[,] orderArray;   //the csv successfully parsed 
        public List<OrderItem> OrderItems { get; set; } //used to pass the list of items
        public MainPage()
        {
            InitializeComponent();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            createLogFolder();

           

        }

        /// <summary>
        /// Author: Jess Merolla
        /// Date: 11/17/2020
        /// NOTE: Pretty sure this is Windows specific atm
        /// 
        /// Checks for a log file for the current date, makes one
        /// if it does not exist.
        /// </summary>
        private async void createLogFolder()
        {
            String dateName = DateTime.Now.ToString("dd-MM-yyyy");
          //  String fileName = dateName;
            IFolder folder = PCLStorage.FileSystem.Current.LocalStorage;

           // Environment.StorageApplicationPermissions.FutureAccessList.Add(Environment
           //  .GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //help
            fileName = Path.Combine(Environment
             .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dateName);

           
            bool doesExist = File.Exists(fileName);
            if (doesExist == true)
            //    if (PCLHelper.IsFolderExistAsync(folderName).Result == true)
            {
                file = folder.GetFileAsync(dateName).Result;
                lbl.Text = "Log file already exists";
            }
            else
            {
                try
                {
                    File.Create(fileName);
                   // IFile fileLog = await folder.CreateFileAsync(dateName, CreationCollisionOption.FailIfExists);
                    lbl.Text = "Log folder created";
                }catch (Exception e)
                {
                    lbl.Text = "Failed to create log folder: " + e.Message;
                }

            }
            

        }


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
                    fileType = "txt";
                }
                if (Device.RuntimePlatform == Device.UWP)
                {
                  
                    fileType = ".txt";
                }

                //Opens file picker
                FileData filedata = await CrossFilePicker.Current.PickFile();

                //Loop file picker until a .csv is selected
                //skips if picking operation is cancelled
                while (filedata!= null && filedata.FileName.Contains(fileType)!= true){
                      lbl.Text = "File Type must be .txt";
                    //  lbl.Text = "File Type must be .csv";
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
                        //lbl.Text = csvdata;
                        ConfirmOrderButton.IsVisible = true; //shows our confirm button after we choose a file
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

            orderText = orderItemParser.extractHeaderInfo(orderText);

            ItemsList = orderText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();


            //OLD CODE FOR CSV
            // int ORDER_COLUMNS = 12;
            // orderArray = orderItemParser.parseOrderItemsIntoArray(ItemsList, ORDER_COLUMNS);
           // OrderItems = orderItemParser.arrayToOrderItemList(orderArray);

            //Parse OrderItems from List (txt version)
            OrderItems = orderItemParser.parseOrderItemsFromList(ItemsList);
            
            return orderText;
        }

        /// @author Jessica Merolla
        /// @date 9/29/2020
        /// 
        /// !!!!!!!!!!! Grab order header
        /// !!!!!!!!!!Toggle visibility in xml if csv file has not been picked
        /// 
        /// <summary>
        /// Passes the string of csv data into the list view,
        /// logd the curret order being packed into the day's log file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmOrderButton_Clicked(object sender, EventArgs e)
        {
            string appendText = "Order Packed: " + Environment.NewLine + csvdata + Environment.NewLine;
            File.AppendAllText(fileName, appendText);

            await Navigation.PushAsync(new OrderListView(OrderItems));

        }
    }
}
