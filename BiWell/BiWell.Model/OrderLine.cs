﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.Model
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int LineNum { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public decimal Amount => Price * Quantity;
    }
}
