using BiWell.Payment.Helpers;
using BiWell.Payment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Implementation
{
    public class CustomerStartKitOrderChecker : IStartKitOrderChecker
    {
        private readonly DateTime _startKitCheckDateFrom = new DateTime(2016, 12, 1);

        public void CheckFor(string custNumber)
        {
            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
            var orderApiCred = orderApiClient.CreateCredentials();

            string[] startKitIds = Properties.Settings.Default.Freedom_StartKitItemId.Split(',');
            bool isStartKitFound = false;

            foreach (var startKitId in startKitIds)
            {
                if (isStartKitFound) break;

                var responseClientDidOrder = orderApiClient.CheckOrderedItemForCustomerDIDWithinDate_V2(
                    orderApiCred,
                    custNumber,
                    startKitId,
                    _startKitCheckDateFrom);

                if (responseClientDidOrder.Success == 0)
                {
                    throw new InvalidOperationException($"Не удается проверить заказы с товаром '{startKitId}' для клиента '{custNumber}'");
                }

                isStartKitFound = (responseClientDidOrder.OrderID > 0); // checking without payment

                //if (!isStartKitFound)
                //{
                //    var orderDetailsResponse = orderApiClient.GetOrderDetailsInfo_V2(orderApiCred, orderId);
                //    if (orderDetailsResponse.Success == 0)
                //    {
                //        throw new InvalidOperationException(orderDetailsResponse.Message);
                //    }

                //    isStartKitFound = orderDetailsResponse.OrderDetailsResponse
                //        .Any(x => x.ProductID.Equals(startKitId));
                //}
            }

            if (!isStartKitFound)
            {
                throw new InvalidOperationException("Стартовый продуктовый набор должен быть заказан");
            }
        }
    }
}