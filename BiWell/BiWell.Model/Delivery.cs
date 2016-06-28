using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.Model
{
    public class Delivery
    {
        public int Id { get; set; }
        public string TrackNum { get; set; }
        public DeliveryStatus Status { get; set; }
        public Order Order { get; set; }
    }
}
