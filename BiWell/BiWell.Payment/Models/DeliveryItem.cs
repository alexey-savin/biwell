using System;
using System.Collections.Generic;
using System.Globalization;
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
        public string FormattedCost => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Cost);
        public string FormattedPayCost => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", PayCost);
        public string FormattedWeight => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Weight);
    }
}