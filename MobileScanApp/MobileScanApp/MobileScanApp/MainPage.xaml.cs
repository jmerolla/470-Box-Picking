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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
