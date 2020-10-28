using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileScanApp
{
    public class OrderItem
    {
        
        public Boolean IsPacked { get; set; }
        public String Name { get; set; }
        public String Location { get; set; }
        public String BarcodeID { get; set; }
        public int PalletQty { get; set; }
        public int CartonQty { get; set; }
        public int QtyOrdered { get; set; }
        public decimal QtyOpen { get; set; }
    }
}
