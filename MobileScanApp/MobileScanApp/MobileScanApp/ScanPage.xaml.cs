using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace MobileScanApp
{
    /*
     *  @author:    Jess Merolla, Graham Hallman-Taylor
     *  @date:      9/18/2020
     *  @summary:
     *
     *  This class will hold the scanner functionality. Takes in a list
     *  created by our OrderSheet, and tells us how many scans we have 
     *  remaining after each successful scan. A successful scan happens 
     *  when the barcode scanned matches the barcode number from the
     *  OrderItem.
     *
     *  @source: https://www.c-sharpcorner.com/blogs/using-zxing-code-128-scanner-in-xamarin-forms2
     *
     *  Last edited: Spencer Dusi 10/15/20
     * 
     */
    public partial class ScanPage : ContentPage
    {
        public String barCodeRead; //Barcode that was scanned
        int qtyScanned = 0; //qty we have already scanned
        int remainingScans = 0; //qty we have left to scan
        Boolean doneScanning = false; //turns true when remaining scans = 0
        bool _isScanning = true; //true before every scan, helps stop multiple scans
        public StackLayout stkMainlayout; //Our ScanPage's layout format
        public OrderItem scannableItem; //holds OrderItem currently being picked
        ZXingScannerPage scanPage;
        public IList<OrderItem> OIList;

        public ScanPage(OrderItem scannableItem, IList<OrderItem> list)
        {
            OIList = list;
            this.scannableItem = scannableItem; //Taking the OrderItem from OrderListView.
            /**
             * Creates a new layout for our ScanPage screen. Things added to this layout
             * will be added to the center of the screen (Horizontally and vertically centered).
             * Each item added (butttons, textboxes, switches, etc..) will be added below the previous
             * item added. 
             */
            stkMainlayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical
            };
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

            var overlay = new ZXingDefaultOverlay();
            overlay.TopText = "Quantity scanned: " + qtyScanned.ToString() + "\r\n\r\n" + "Quantity remaining: " + scannableItem.QtyOrdered.ToString();
            overlay.BottomText = "Located in Section: " + scannableItem.Location;
            overlay.ShowFlashButton = true;
            overlay.FlashButtonClicked += (t, ed) =>
            {
                scanPage.ToggleTorch();
            };
            overlay.BindingContext = overlay; //sets my overlay with intended objects
            //Once button is clicked, create a new scanpage with the options and overlay set above
            btnScan.Clicked += async (a, s) =>
            {
                scanPage = new ZXingScannerPage(options, overlay);
        //Once we capture a barcode we "BeginInvokeOnMainThread" to check what we scanned
        scanPage.OnScanResult += (result) =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (_isScanning)
                        {
                            _isScanning = false; //bool to stop _isScanning from allowing us to enter if() statement.
                            scanPage.IsAnalyzing = false; //stops scanning barcodes once this is implemented.
                            barCodeRead = result.Text; //sets barcode scanned in as a string
                            if (barCodeMatcher()) //If the scan matches the barcode from the OrderItem list, display the "Barcode Matches" alert.
                            {
                                await DisplayAlert("Barcode Matches", result.Text + " , " + " Remaining Scans: " + RemainingScans() + " , " + "QtyScanned: " + qtyScanned.ToString(), "OK");
                                overlay.TopText = "Quantity scanned: " + qtyScanned.ToString() + "\r\n\r\n" + "Quantity remaining: " + remainingScans.ToString(); //overlay to represent how many scans remain.
                                if (doneScanning)
                                {
                                    await Navigation.PopModalAsync(); //Takes us back to the page with the scan button to know we are done.
                                    await DisplayAlert("Finished Scanning: ", scannableItem.Name + " is completed.", "OK"); //Alert to know we are done scanning an item.                                
                                    await Navigation.PopAsync(); //Takes us back to the page where we choose which item we are about to scan.
                                    OIList.Remove(scannableItem); //Removes this item from the list since the correct quantity of it has now been scanned
                                    OIList = list; //Replaces the old list with the new list that does not contain the finished item 
                                }
                            }
                            else
                            {
                                await DisplayAlert("Scanned Barcode", result.Text + " , " + result.BarcodeFormat + " ," + result.ResultPoints[0].ToString() + " , " + " Barcode does NOT match the one from the Order List.", "OK"); //Every barcode scanned that does not match will display as an alert.
                            }
                                scanPage.IsAnalyzing = true; //Allows us to scan again once we "ok" the popup.
                                _isScanning = true; //Allows us to be able to reenter our if() statement.
                            }
                    });
                };
                await Navigation.PushModalAsync(scanPage); //Takes us to the page where we see what the camera is picking up
            };
            stkMainlayout.Children.Add(btnScan); //adds button to our screen
            Content = stkMainlayout; //sets our content in the .cs instead of .xaml
        }

        /**
         * Edited by Spencer Dusi 9-29-20.
         * This Method takes the barcodes and test to see if it matches the one on the sheet.
         * If it matches we set our boolean true allowing the pop up to happen.
         * DONE - Link the barcode numbers from the list.
         * @Return - true if the barcode scanned matches the BarcodeID from OrderItem. false otherwise.
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
         * @Return - remaining amount of scans left on an item
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
            remainingScans = qtyOrdered - qtyScanned;
            return remainingScans;
        }
    }
}
