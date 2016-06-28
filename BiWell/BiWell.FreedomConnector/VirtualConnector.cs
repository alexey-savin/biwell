using BiWell.FreedomConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiWell.Model;

namespace BiWell.FreedomConnector
{
    public class VirtualConnector : IConnector
    {
        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            Order o1 = new Order
            {
                Id = 1,
                Number = 800100,
                Date = DateTime.Today,
            };

            o1.AddLine(
                new OrderLine
                {
                    Id = 1,
                    LineNum = 1,
                    Name = "order line #1",
                    Price = 12.0m,
                    Quantity = 1
                });
            o1.AddLine(
                new OrderLine
                {
                    Id = 2,
                    LineNum = 2,
                    Name = "order line #2",
                    Price = 13.55m,
                    Quantity = 4
                });

            Order o2 = new Order
            {
                Id = 2,
                Number = 800102,
                Date = DateTime.Today.AddDays(-2),
            };

            o2.AddLine(
                new OrderLine
                {
                    Id = 3,
                    LineNum = 1,
                    Name = "order line #1",
                    Price = 9.99m,
                    Quantity = 2
                });
            o2.AddLine(
                new OrderLine
                {
                    Id = 4,
                    LineNum = 2,
                    Name = "order line #2",
                    Price = 13.79m,
                    Quantity = 3
                });
            o2.AddLine(
                new OrderLine
                {
                    Id = 5,
                    LineNum = 3,
                    Name = "order line #3",
                    Price = 7.79m,
                    Quantity = 12
                });

            orders.Add(o1);
            orders.Add(o2);

            return orders;
        }
    }
}
