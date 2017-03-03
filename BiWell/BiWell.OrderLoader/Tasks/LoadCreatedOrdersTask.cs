using BiWell.OrderLoader.ByDesignOrderApi;
using System;
using System.Diagnostics;

namespace BiWell.OrderLoader.Tasks
{
    public class LoadCreatedOrdersTask : ITask
    {
        public void Run()
        {
            var orderApiClient = new OrderAPISoapClient();

            var cred = new Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            var responseOrderList = orderApiClient.GetOrderListRecent(
                cred,
                Properties.Settings.Default.Freedom_RecentPeriodType,
                Properties.Settings.Default.Freedom_RecentPeriodLength,
                false);

            using (var dbContext = new BiWellEntities())
            {
                dbContext.Database.Log = s => Debug.WriteLine(s);

                foreach (var order in responseOrderList)
                {
                    var dbOrder = dbContext.order_table.Find(order.OrderID);
                    if (dbOrder == null)
                    {
                        var responseOrderInfo = orderApiClient.GetOrderInfo_V2(cred, order.OrderID);
                        if (responseOrderInfo.Success == 0)
                        {
                            throw new InvalidOperationException(responseOrderInfo.Message);
                        }

                        dbOrder = new order_table
                        {
                            order_id = order.OrderID,
                            created_at = order.CreatedDate,
                            modified_at = order.LastModifiedDate,
                            status = responseOrderInfo.Status,
                            shipping_method_id = responseOrderInfo.ShipMethodID
                        };

                        if (dbOrder.modified_at == DateTime.MinValue)
                        {
                            dbOrder.modified_at = null;
                        }

                        dbContext.order_table.Add(dbOrder);
                    }
                }

                dbContext.SaveChanges();
            }
        }
    }
}
