﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MobileScanApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();
            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingBarcodeImageViewRenderer.Init();
            LoadApplication(new MobileScanApp.App());
        }
    }
}
