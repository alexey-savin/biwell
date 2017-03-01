using BiWell.OrderLoader.ByDesignOrderApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.OrderLoader.Tasks
{
    public class LoadUpdatedOrdersTask : ITask
    {
        public void Run()
        {
            var orderApiClient = new OrderAPISoapClient();

            var cred = new Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            var requestOrderListRecent = new GetOrderListRecentRequest();
            requestOrderListRecent.Credentials = cred;
            var responseOrderList = orderApiClient.GetOrderListRecent(
                cred,
                Properties.Settings.Default.Freedom_RecentPeriodType,
                Properties.Settings.Default.Freedom_RecentPeriodLength,
                true);

            using (var dbContext = new BiWellEntities())
            {
                foreach (var order in responseOrderList)
                {
                    dbContext.Database.Log = s => Debug.WriteLine(s);

                    var dbOrder = dbContext.order_table.Find(order.OrderID);
                    if (dbOrder != null)
                    {
                        var responseOrderInfo = orderApiClient.GetOrderInfo_V2(cred, order.OrderID);
                        if (responseOrderInfo.Success == 0)
                        {
                            throw new InvalidOperationException(responseOrderInfo.Message);
                        }

                        dbOrder.status = responseOrderInfo.Status;
                        dbOrder.modified_at = order.LastModifiedDate;

                        if (dbOrder.modified_at == DateTime.MinValue)
                        {
                            dbOrder.modified_at = null;
                        }
                    }
                }

                dbContext.SaveChanges();
            }
        }
    }
}
