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
                    });
                };
                await Navigation.PushModalAsync(scanPage);
            };
            stkMainlayout.Children.Add(btnScan);
            Content = stkMainlayout;
        }
    }
}
