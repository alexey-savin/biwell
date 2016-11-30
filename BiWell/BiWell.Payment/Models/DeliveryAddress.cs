using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Models
{
    public class DeliveryAddress
    {
        public string PostIndex { get; set; }
        public string Place { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Appartment { get; set; }
    }
}