using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileScanApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class OrderListView : ContentPage
    {
        IList<OrderItem> OrderItems { get;  set; }
        public List<String> ParsedCSV;


        /// @author Jess Merolla
        /// @date   9/30/2020
        /// <summary>
        /// Passes in a string of csv data and parse it to populate a list of order items
        /// </summary>
        /// <param name="csvdata">String holding the csv data</param>
        public OrderListView(List<String> items)
        {
            InitializeComponent();
            ParsedCSV=items;

            //TODO would probably call a method for parsing here
            //maybe parse into multiple arrays and cycle through them in a loop
            //filling up OrderItems?

            OrderItems = new List<OrderItem>();
            //TODO replace testers with parsed csv info
            OrderItems.Add(new OrderItem
            {
                Name = "Test1",
                Location = "Aisle 1",
                BarcodeID = "88874",
                PalletQty = 1,
                CartonQty = 5
            });
            OrderItems.Add(new OrderItem
            {
                Name = "Test2",
                Location = "Aisle 2",
                BarcodeID = "11111",
                PalletQty = 2,
                CartonQty = 10
            });
            MyListView.ItemsSource = OrderItems;
            //BindingContext = this;
        }

        /// <summary>
        /// 
        /// @author Jess Merolla
        /// @date 9/30/2020
        /// TODO modify to scan in item? Or to check it off?
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
