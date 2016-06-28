using BiWell.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.DataAccess.Interfaces
{
    public interface IRepository
    {
        List<Order> GetOrders();
    }
}
