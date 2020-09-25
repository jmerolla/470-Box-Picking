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
        
        public MainPage()
        {
            InitializeComponent();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;


            //test to get a feel for a potential logo header
            BoxView logoBoxview = new BoxView
            {
                Color = Color.Accent,
                WidthRequest = mainDisplayInfo.Width,
                HeightRequest = mainDisplayInfo.Height / 12,
            };

         
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
         * @author Jess Merolla
         * 
         * Calls a file picker and allows the user to pick a file that will be used for parsing
         * 
         * !!!!!!!!!!!TO-DO add file type restrictions, do something with the file
         */
        private async void pickFileButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                FileData filedata = await CrossFilePicker.Current.PickFile();

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
