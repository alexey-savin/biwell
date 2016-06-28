using BiWell.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.FreedomConnector.Interfaces
{
    public interface IConnector
    {
        List<Order> GetOrders();
    }
}
