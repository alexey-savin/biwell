using System;
using System.Collections.Generic;

namespace BiWell.Payment.Models
{
    public class DeliveryParameters
    {
        public string ticket { get; set; }
        public string ver => "1";
        public string callbackURL { get; set; }

        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int ShipMethodId { get; set; }
        public string ShipMethod { get; set; }

        public DeliveryItem[] Items { get; set; }
        public DeliveryAddress Address { get; set; }
        public DeliveryRecipient Recipient { get; set; }

        public string ItemsString
        {
            get
            {
                var itemsStr = new List<string>();
                foreach (var item in Items)
                {
                    itemsStr.Add($"{item.ItemId}/{item.Quantity}/{item.Cost}");
                }

                return string.Join(",", itemsStr);
            }
        }
    }
}