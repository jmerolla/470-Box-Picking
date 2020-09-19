using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

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


            BoxView logoBoxview = new BoxView
            {
                Color = Color.Accent,
                WidthRequest = mainDisplayInfo.Width,
                HeightRequest = mainDisplayInfo.Height / 12,
            };

            this.Content = new StackLayout
            {
                Children =
               {
                   logoBoxview,
                   scanOrderButton
               }
            };
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Title", "Hello World", "OK");
        }
    }
}
