using System;
using System.Collections.Generic;
using System.Text;

namespace MobileScanApp
{
    public class OrderItem
    {
        public String Name { get; set; }
        public String Location { get; set; }
        public String BarcodeID { get; set; }
        public int PalletQty { get; set; }
        public int CartonQty { get; set; }

    }
}
