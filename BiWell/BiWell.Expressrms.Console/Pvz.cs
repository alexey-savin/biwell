using System.Collections.Generic;

namespace BiWell.Expressrms.Console
{
    public class Pvz
    {
        public List<PointDelivery> Points { get; set; }
    }

    public class PointDelivery
    {
        public string DeliveryService { get; set; }
        public string Code { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
