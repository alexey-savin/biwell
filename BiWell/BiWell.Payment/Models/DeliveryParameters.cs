﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Models
{
    public class DeliveryParameters
    {
        public string ticket { get; set; }
        public string ver => "1";
        public string callbackURL => "about:blank";
    }
}