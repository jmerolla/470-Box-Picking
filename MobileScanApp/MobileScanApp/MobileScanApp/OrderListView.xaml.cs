using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileScanApp
{
    //Added headings to the listview in OrderListView.xaml
    //TODO: Qty opened not appearing    
    //Edited: 10/25/20 by Spencer Dusi
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class OrderListView : ContentPage
    {
        public IList<OrderItem> OrderItems { get;  set; }
        IEnumerable myCol;
        ObservableCollection<OrderItem> myCollection;
        /// @author Jess Merolla
        /// @date   9/30/2020
        /// <summary>
        /// Passes in a string of csv data and parse it to populate a list of order items
        /// </summary>
        /// <param name="csvdata">String holding the csv data</param>
        public OrderListView(List<OrderItem> OrderItems)
        {
            InitializeComponent();
            this.OrderItems = OrderItems;
            //This is a fake example using my deoderant barcode
            //REMOVE THESE WHEN TESTING IS DONE!!!
            //SAMPLE ITEMS FOR SCANNING
            /*
            //Graham's package of nails
            OrderItems.Add(new OrderItem
            {
                IsPacked = false,
                Name = "nail example",
                LocationQOH = "s16 s12",
                BarcodeID = "191518759372",
                PalletQty = 2,
                CartonQty = 10,
                QtyOrdered = 1,
                QtyOpen = 2,
                UM = "RL",
                ExtPrice = "20.00",
                DueDate = "test"
            });
            */
            myCollection = new ObservableCollection<OrderItem>(OrderItems);
            this.Content = Content; //sets our content from our OrderListView.xaml
            MyListView.ItemsSource = myCollection; //sets all of our items in our listview
            myCol = myCollection; //temp IEnumerable for timer.
        }

        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Boolean allPacked = true;
                MyListView.ItemsSource = null; //Moved itemsource here from onTimerElapsed.
                MyListView.ItemsSource = myCol;//keep comment for bug

                foreach (var items in myCollection)
                {
                    if (items.IsPacked == false)
                    {
                        allPacked = false;
                    }
                }

                if (allPacked == true)   //all items have been packed
                {
                    await DisplayAlert("Order Complete", "You have scanned all items in this order", "OK");
                    returnToMainPage();
                }
            });
        }

        /*
         * Once our timer runs out call this method that refreshes our table to 
         * myCollection and disable the timer from restarting so it does not keep refreshing.
         * 
         *  TODO: (@author Jess) add a check to see if scanning is complete for all items and then return to main page
         */

        /// <summary>
        /// Navigates back to the main page to select a new Order file
        /// </summary>
        async void returnToMainPage()
        {
            try
            {
                await Navigation.PopAsync();    //return to MainPage.xaml.cs
                //await Navigation.PushAsync(new MainPage());
            }
            catch (Exception f)
            {
                await DisplayAlert("Return to MainPage failed", f.Message, "OK");
            }
        }
        /// <summary>
        /// 
        /// @author Jess Merolla, Graham Hallman-Taylor
        /// @date 9/30/2020
        /// 
        /// When an OrderItem is tapped, that OrderItem's contents are sent to a new 
        /// 
        /// </summary>
        /// <param name="sender">Used to access the currently selected OrderItem</param>
        /// <param name="e">Event handler for item tapping</param>
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            {
                if (((OrderItem)e.Item).IsPacked == true)
                {
                    await DisplayAlert("Item already completed", "Enough of this item has already been scanned in for this order.", "OK");
                    ((ListView)sender).SelectedItem = false; //Deselect Item
                    return;
                }
                else
                {
                    await Navigation.PushAsync(new ScanPage((OrderItem)((ListView)sender).SelectedItem, OrderItems));
                }
                ((ListView)sender).SelectedItem = false; //Deselect Item
            }
    }
}
