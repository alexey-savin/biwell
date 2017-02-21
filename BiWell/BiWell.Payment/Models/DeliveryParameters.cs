using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        public decimal ShippingTotal { get; set; }

        public DeliveryItem[] Items { get; set; }
        public Address DeliveryAddress { get; set; }
        public DeliveryRecipient Recipient { get; set; }
        public ClientContactInfo ContactInfo { get; set; }

        public string ItemsString
        {
            get
            {
                var startKitItems = Properties.Settings.Default.Freedom_StartKitItemId.Split(',');

                var itemsStr = new List<string>();
                foreach (var item in Items)
                {
                    if (!startKitItems.Contains(item.ItemId))
                    {
                        itemsStr.Add($"{item.ItemId}/{item.Quantity}/{string.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Cost)}");
                    }
                }

                return string.Join(",", itemsStr);
            }
        }

        public string ItemsStringWithCost
        {
            get
            {
                var itemsStr = new List<string>();
                foreach (var item in Items.Where(x => x.Cost > 0))
                {
                    itemsStr.Add($"{item.ItemId}/{item.Quantity}/{string.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Cost)}");
                }

                return string.Join(",", itemsStr);
            }
        }

        public bool IsSelfPickup => ShipMethodId == SelfPickupShipMethodId;

        public int SelfPickupShipMethodId => Properties.Settings.Default.Freedom_SelfPickupShipMethodId;

        public bool IsPosted => Status == "Posted";

        public string DeliveryCode
        {
            get
            {
                var result = string.Empty;

                if (ShipMethod.StartsWith("Пункт выдачи") || ShipMethod.StartsWith("Постамат"))
                {
                    result = "104.1.0";
                }
                else if (ShipMethod.StartsWith("Курьер"))
                {
                    result = "104.2.1";
                }
                else if (ShipMethod.StartsWith("Почта"))
                {
                    result = "112.4.1";
                }

                return result;
            }
        }
    }
}