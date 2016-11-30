using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Models
{
    public class DeliveryItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal PayCost { get; set; }
        public decimal Weight { get; set; }
    }
}