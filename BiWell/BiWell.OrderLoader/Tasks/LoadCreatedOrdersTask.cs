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

            var requestOrderListRecent = new GetOrderListRecentRequest();
            requestOrderListRecent.Credentials = cred;
            var responseOrderList = orderApiClient.GetOrderListRecent(
                cred,
                Properties.Settings.Default.Freedom_RecentPeriodType,
                Properties.Settings.Default.Freedom_RecentPeriodLength,
                false);

            using (var dbContext = new BiWellEntities())
            {
                foreach (var order in responseOrderList)
                {
                    dbContext.Database.Log = s => Debug.WriteLine(s);

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
                            status = responseOrderInfo.Status,
                            created_at = order.CreatedDate,
                            modified_at = order.LastModifiedDate
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
