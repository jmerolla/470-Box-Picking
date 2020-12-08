using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileScanApp
{
    /// <summary>
    /// @author Jess Merolla
    /// @date 10/14/2020
    /// 
    /// Contains the different fields that an Order Item will hold
    /// </summary>
    public class OrderItem
    {
        
        public Boolean IsPacked { get; set; }
        public String Name { get; set; }
        public String LocationQOH { get; set; }
        public String BarcodeID { get; set; }
        public decimal PalletQty { get; set; }
        public decimal CartonQty { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal QtyOpen { get; set; }
        public String UM { get; set; }
        public String ExtPrice { get; set; }
        public String DueDate { get; set; }
    }
}
