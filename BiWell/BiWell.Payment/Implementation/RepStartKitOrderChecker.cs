﻿using BiWell.Payment.ByDesignOrderAPI;
using BiWell.Payment.Helpers;
using BiWell.Payment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiWell.Payment.Implementation
{
    public class RepStartKitOrderChecker : IStartKitOrderChecker
    {
        private readonly DateTime _startKitCheckDateFrom = new DateTime(2016, 12, 1);

        public void CheckFor(string repNumber)
        {
            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
            var orderApiCred = orderApiClient.CreateCredentials();

            string[] startKitIds = Properties.Settings.Default.Freedom_StartKitItemId.Split(',');
            bool isStartKitFound = false;

            foreach (var startKitId in startKitIds)
            {
                if (isStartKitFound) break;

                var orderListRangeRequest = new GetOrderListRangeRequest(orderApiCred, _startKitCheckDateFrom, DateTime.Now, null);
                var responseOrderListRange = orderApiClient.GetOrderListRange(orderListRangeRequest);

                foreach (var orderList in responseOrderListRange.GetOrderListRangeResult
                    .OrderBy(x => x.OrderID))
                {
                    if (orderList.Success > 0)
                    {
                        var responseOrderInfo = orderApiClient.GetOrderInfo_V2(orderApiCred, orderList.OrderID);
                        if (responseOrderInfo.Success == 0)
                        {
                            throw new InvalidOperationException(responseOrderInfo.Message);
                        }

                        if (responseOrderInfo.RepNumber.Equals(repNumber))
                        {
                            var responseOrderDetails = orderApiClient.GetOrderDetailsInfo_V2(orderApiCred, orderList.OrderID);
                            if (responseOrderDetails.Success == 0)
                            {
                                throw new InvalidOperationException(responseOrderDetails.Message);
                            }

                            isStartKitFound = responseOrderDetails.OrderDetailsResponse
                                .Any(x => x.ProductID.Equals(startKitId));

                            if (isStartKitFound) break;
                        }
                    }
                }
            }

            if (!isStartKitFound)
            {
                throw new InvalidOperationException("Стартовый продуктовый набор должен быть заказан");
            }
        }
    }
}