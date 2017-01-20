using System;
using static System.Console;

namespace BiWell.OrderAPI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ByDesignWebService.Credentials cred = new ByDesignWebService.Credentials();
            cred.Username = "AlexeyInkuev";
            cred.Password = "Moll35$#";
            
            ByDesignWebService.OrderAPISoap orderApiClient = new ByDesignWebService.OrderAPISoapClient();

            var requestOrderListRecent = new ByDesignWebService.GetOrderListRecentRequest();
            requestOrderListRecent.Credentials = cred;
            requestOrderListRecent.PeriodType = "day";
            requestOrderListRecent.PeriodLength = 10;
            requestOrderListRecent.EvalDateLastModified = false;
            var responseOrderList = orderApiClient.GetOrderListRecent(requestOrderListRecent);

            DateTime start = DateTime.Now;
            foreach (var orderList in responseOrderList.GetOrderListRecentResult)
            {
                var responseOrderInfo = orderApiClient.GetOrderInfo_V2(cred, orderList.OrderID);
                if (responseOrderInfo.Success == 0)
                {
                    continue;
                }

                WriteLine($"{orderList.OrderID} = {responseOrderInfo.Status}, {responseOrderInfo.ShipMethod}");
            }

            WriteLine($"Done with {DateTime.Now.Subtract(start).TotalSeconds}");
            ReadKey();
        }
    }
}
