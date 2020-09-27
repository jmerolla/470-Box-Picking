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
        private async void NavigateToScanButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScanPage());
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

                //display file name
                if (filedata != null)
                {
                    lbl.Text = filedata.FileName;

                }
                
                //Prints all values of CSV to console
                if(filedata != null)
				{
                    var filePath = filedata.FilePath;
                    if (filedata != null)
                    {
                        lbl.Text = filedata.FileName;
                        String csvdata = String.Join(", ", ReadInCSV(filePath).ToArray());
                        System.Diagnostics.Debug.Write(csvdata);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /*
        * @author Graham
        * 9/27/2020
        * Reads in a CSV using a given path and gives a full list of values in a List<String>
        * *To-do* Only get relevant data + more documentation
        */
        public static List<string> ReadInCSV(string absolutePath)
        {
            List<string> result = new List<string>();
            string value;
            using (TextReader fileReader = File.OpenText(absolutePath))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = false;
                while (csv.Read())
                {
                    for (int i = 0; csv.TryGetField<string>(i, out value); i++)
                    {
                        result.Add(value);
                    }
                }
            }
            return result;
        }
    }
}
