using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
            OrderItems.Add(new OrderItem
            {
                Name = "1134HP Acrylic 0.5 mil foil",
                Location = "r5",
                BarcodeID = "012044038918",
                PalletQty = 1,
                CartonQty = 5,
                QtyOrdered = 2,
                QtyOpen = 2
            });
            //This is an actual example from the box from AD
            OrderItems.Add(new OrderItem
            {
                Name = "0808HP Acrylic 2.0 mil foil",
                Location = "s16",
                BarcodeID = "655616007419",
                PalletQty = 2,
                CartonQty = 10,
                QtyOrdered = 432,
                QtyOpen = 432
            });

            
            MyListView.ItemsSource = OrderItems;
            //BindingContext = this;
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
            if (e.Item == null)
            {
                await DisplayAlert("Item already completed", "Enough of this item has already been scanned in for this order.", "OK");
                return;
            }
            else
            {
                await Navigation.PushAsync(new ScanPage((OrderItem)((ListView)sender).SelectedItem, OrderItems));
            }
        ((ListView)sender).SelectedItem = null; //Deselect Item
        }
    }
}
