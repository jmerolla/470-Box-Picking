using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;
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
        int qtyScanned = 0;
        Boolean doneScanning = false;
        private bool _isScanning = true;
        StackLayout stkMainlayout;
        OrderItem scannableItem; //holds OrderItem currently being picked

        public ScanPage(OrderItem scannableItem)
        {
            this.scannableItem = scannableItem;
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
            /**
             *  var "options" allows you to choose what options you want your scanner to
             * allow. Currently using it to AutoRotate and to "TryHarder" which 
             * gets or sets a flag which cause a deeper look into the bitmap.This
             * just makes it so the camera focuses on the barcode quicker. Though it
             * does add room for misscans. 
             */
            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = true,
                UseFrontCameraIfAvailable = false,
                TryHarder = false
            };
            btnScan.Clicked += async (a, s) =>
            {
                scanPage = new ZXingScannerPage(options);
                scanPage.OnScanResult += (result) =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (_isScanning)
                        {
                            _isScanning = false;
                            scanPage.IsAnalyzing = false;
                            await DisplayAlert("Scanned Barcode", result.Text + " , " + result.BarcodeFormat + " ," + result.ResultPoints[0].ToString(), "OK");
                            barCodeRead = result.Text;
                            if (barCodeMatcher()) //If the scan matches the barcode from the OrderItem list, display the alert.
                            {
                                await DisplayAlert("Barcode Matches", result.Text + " , " + " Remaining Scans: " + RemainingScans() + " , " + "QtyScanned: " + qtyScanned.ToString(), "OK");
                                if (doneScanning)
                                {
                                    await Navigation.PopModalAsync(); //Takes us back to the page with the scan button to know we are done.
                                    await DisplayAlert("Finished Scanning: ", scannableItem.Name + " is completed.", "OK");
                                }
                            }
                            scanPage.IsAnalyzing = true;
                            _isScanning = true;
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
         * This Method takes the barcodes and test to see if it matches the one on the sheet.
         * If it matches we set our boolean true allowing the pop up to happen.
         * DONE - Link the barcode numbers from the list.
         */
        public Boolean barCodeMatcher()
        {
            if (scannableItem.BarcodeID == barCodeRead)
            {
                return true;
            }
            return false;
        }

        /**
         * Edited by Spencer Dusi 10-12-20.
         * This method takes the quantity ordered for the specific item
         * and each time the item is scanned it adds to the counter until
         * it has been scanned the amount of times the quantity desires.
         */
        public int RemainingScans()
        {
            int qtyOrdered = scannableItem.QtyOrdered;
            if (doneScanning != true)
            {
                qtyScanned++;
            }   
            if (qtyOrdered >= qtyScanned)
            {
                if (qtyOrdered == qtyScanned)
                {
                    doneScanning = true;
                }
                else
                {
                    doneScanning = false;
                }
            }
            return qtyOrdered - qtyScanned;
        }
    }
}
