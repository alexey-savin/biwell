using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.Model
{
    public class Order
    {
        private List<OrderLine> _lines = null;

        public Order()
        {
            _lines = new List<OrderLine>();
        }

        public int Id { get; set; }
        public long Number { get; set; }
        public DateTime Date { get; set; }
        public Delivery Delivery { get; set; }
        public List<OrderLine> Lines => _lines;
        public void AddLine(OrderLine line)
        {
            _lines.Add(line);
            line.Order = this;
        }
        public void SetDelivery(Delivery delivery)
        {
            Delivery = delivery;
            delivery.Order = this;
        }
        public decimal Amount => _lines.Sum(line => line.Amount);
        public bool IsDelivery => Delivery != null;
    }
}
