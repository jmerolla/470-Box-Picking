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
        public String[] Location { get; set; }
        public String BarcodeID { get; set; }
        public decimal PalletQty { get; set; }
        public decimal CartonQty { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal QtyOpen { get; set; }
    }
}
