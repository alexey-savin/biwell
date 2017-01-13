using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Models
{
    public class Address
    {
        public string PostIndex { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Place { get; set; }
        public string Street_1 { get; set; }
        public string Street_2 { get; set; }
        public string House { get; set; }
        public string Appartment { get; set; }
    }
}