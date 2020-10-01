using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace MobileScanApp
{
    /*
     *  @author:    Jess Merolla
     *  @date:      9/18/2020
     *  @summary:
     *
     *  This page will hold the scanner functionality
     *
     *  @source: https://www.c-sharpcorner.com/blogs/using-zxing-code-128-scanner-in-xamarin-forms2
     *
     *
     *
     */
    public partial class ScanPage : ContentPage
    {
        String barCodeRead;
        StackLayout stkMainlayout;
        public ScanPage()
        {
            stkMainlayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical
            };
            ZXingScannerPage scanPage;
            Button btnScan = new Button
            {
                Text = "Scan",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            btnScan.Clicked += async (a, s) => {
                scanPage = new ZXingScannerPage();
                scanPage.OnScanResult += (result) => {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () => {
                        await Navigation.PopModalAsync();
                        await DisplayAlert("Scanned Barcode", result.Text + " , " + result.BarcodeFormat + " ," + result.ResultPoints[0].ToString(), "OK");
                            barCodeRead = result.Text;
                        if (barCodeMatcher()) //If the scan matches the barcodes, display the alert.
                        {
                            await DisplayAlert("Barcode Matches", result.Text + " , " + result.BarcodeFormat + " ," + result.ResultPoints[0].ToString(), "OK");
                        }
                    });
                };
                await Navigation.PushModalAsync(scanPage);
            };
            stkMainlayout.Children.Add(btnScan);
            Content = stkMainlayout;
          
        }

        /**
         * Edited by Spencer Dusi 9-29-20.
         * This Class takes the barcodes and test to see if it matches the one on the sheet.
         * If it matches we set our boolean true allowing the pop up to happen.
         * TODO - Link the barcode numbers from the list.
         */
        public Boolean barCodeMatcher() {
            var BarCodes = new List<string>()
                {
                   "5561600741",
                   "1204403891",
                   "9151875937",
                };
            //Testing to see if the result of the capture match our barcodes.
            //Loops through the array of barcodes scanned in. If true the pop up activates.
            for (int i = 0; i < BarCodes.Count; i++) {
                if (barCodeRead.Contains(BarCodes[i]))
                {
                    return true;
                }
            }
                return false;
        }
    }
}
