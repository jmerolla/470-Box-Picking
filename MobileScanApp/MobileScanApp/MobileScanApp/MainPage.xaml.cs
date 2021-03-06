﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using PCLStorage;
using System.IO;

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
     */
    public partial class MainPage : ContentPage
    {
        //Used to refernece the log file
        string fileName;
        IFile file;
        String orderHeader; //holds the non-item information from the order file
        String orderData;//make the info globally accessible
        List<String> ItemsList = new List<String>();

        TextFileHandler orderItemParser = new TextFileHandler();    //used to parse the order sheets
        public List<OrderItem> OrderItems { get; set; } //used to pass the list of items
        public MainPage()
        {
            InitializeComponent();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            createLogFile();
        }

        /// <summary>
        /// Author: Jess Merolla
        /// Date: 11/17/2020
        /// NOTE: Pretty sure this is Windows specific atm
        /// 
        /// Checks for a log file for the current date, makes one
        /// if it does not exist.
        /// </summary>
        private async void createLogFile()
        {
            String dateName = DateTime.Now.ToString("dd-MM-yyyy");
            IFolder folder = PCLStorage.FileSystem.Current.LocalStorage;

            fileName = Path.Combine(Environment
             .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dateName);

            bool doesExist = File.Exists(fileName);
            if (doesExist == true)  //file exists, grab it
            {
                try
                {
                    //Must be async or file selection will fail
                    file = (await folder.GetFileAsync(dateName));//.Result;
                    lbl.Text = "Log file already exists";
                }
                catch (Exception e)
                {
                    lbl.Text = "Failed to grab log file: " + e.Message;
                }
            }
            else
            {
                //file doesn't exist, create it
                {
                    try
                    {
                        var myFile = File.Create(fileName);
                        myFile.Close(); //must close the filestream to access the file for the first time
                        lbl.Text = "Log file created";
                    }
                    catch (Exception e)
                    {
                        lbl.Text = "Failed to create log file: " + e.Message;
                    }

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
         * Graham added a call to ReadInTXT at the end of this method 9/27/2020 
         * 
         */
        private async void PickFileButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Specifies text file type for each platform
                string fileType = null;
                if (Device.RuntimePlatform == Device.UWP)
                {
                    fileType = ".txt";
                }
                //Opens file picker
                FileData filedata = await CrossFilePicker.Current.PickFile();

                //Loop file picker until a text file is selected
                //skips if picking operation is cancelled
                while (filedata!= null && filedata.FileName.Contains(fileType)!= true){
                      lbl.Text = "File Type must be .txt";
                    filedata = await CrossFilePicker.Current.PickFile();
                }
                //Prints all values of order data to console
                if(filedata != null)
				{
                    lbl.Text = filedata.FileName;
                    var filePath = filedata.FilePath;
                    if (filedata != null)
                    {
                        lbl.Text = filedata.FileName;
                        orderData = ReadInTXT(filedata);
                        System.Diagnostics.Debug.Write(orderData);
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
        * Reads in a .txt using a given path and gives a full list of values in a List<String>
        * TODO: -pass parse array info into a list of OrderItems and pass those to OrderListView.xaml.cs
        *       -DONE 10/15 -make the parsing stuff into its own class that gets called in the read CSV method
        *       -DONE 10/10 - Jess figure out how to minimize the blank spaces at the end of the array
        */
        public String ReadInTXT(FileData filedata)
        {
            StreamReader reader = new StreamReader(filedata.GetStream());
            string orderText = reader.ReadToEnd();

            orderHeader = orderItemParser.getOrderHeader(orderText); 
            orderText =  orderItemParser.removeEndOfOrder(orderText);
            orderText = orderItemParser.removeHeaderInfo(orderText);

            ItemsList = orderText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            OrderItems = orderItemParser.parseOrderItemsFromList(ItemsList); 

            return orderText;
        }

        /// @author Jessica Merolla
        /// @date 9/29/2020
        /// <summary>
        /// Passes the string of order data into the list view,
        /// logd the curret order being packed into the day's log file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmOrderButton_Clicked(object sender, EventArgs e)
        {
            string appendText = "Order Packed: " + Environment.NewLine + orderHeader
                 + Environment.NewLine +  orderData + Environment.NewLine;
            File.AppendAllText(fileName, appendText);

            await Navigation.PushAsync(new OrderListView(OrderItems));
        }
    }
}
