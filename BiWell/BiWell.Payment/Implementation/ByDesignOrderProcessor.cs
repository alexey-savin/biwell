using BiWell.Payment.Helpers;
using System;
using BiWell.Payment.Interfaces;
using BiWell.Payment.Models;
using System.Linq;

namespace BiWell.Payment.Implementation
{
    public class ByDesignOrderProcessor : IPaymentNotificationProcessor
    {
        private const string CustomerType_Regular = "Reg";
        private const string CustomerType_Preferred = "PC";

        private readonly IExceptionLogger _looger = null;
        
        public ByDesignOrderProcessor(IExceptionLogger logger)
        {
            _looger = logger;
        }

        public void ProcessNotification(PaymentNotificationData data)
        {
            try
            {
                int orderId = 0;
                if (int.TryParse(data.mnt_transaction_id, out orderId))
                {
                    var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
                    var orderApiCred = orderApiClient.CreateCredentials();
                    var response = orderApiClient.GetOrderInfo_V2(orderApiCred, orderId);

                    if (response.Success > 0)
                    {
                        int custNum = 0;
                        if (int.TryParse(response.CustomerNumber, out custNum))
                        {
                            if (custNum >= 2000)
                            {
                                var orderDetailsResponse = orderApiClient.GetOrderDetailsInfo_V2(orderApiClient.CreateCredentials(), orderId);
                                if (orderDetailsResponse.Success == 0)
                                {
                                    throw new InvalidOperationException(orderDetailsResponse.Message);
                                }

                                if (orderDetailsResponse.OrderDetailsResponse
                                    .Any(x => x.ProductID.Equals(Properties.Settings.Default.Freedom_VipKitItemId)))
                                {
                                    // change customer type
                                    var onlineApiClient = ByDesignAPIHelper.CreateOnlineAPIClient();
                                    var onlineApiCred = onlineApiClient.CreateCredentials();

                                    var types = onlineApiClient.GetCustomerTypes(onlineApiCred);
                                    var preferredType = types.FirstOrDefault(x => x.Abbreviation.Equals(CustomerType_Preferred));
                                    if (preferredType != null)
                                    {
                                        onlineApiClient.SetCustomerType(onlineApiCred, response.CustomerNumber, preferredType.ID);
                                    }
                                }
                            }
                        }

                        OrderStatus currentStatus = OrderStatus.Unknown;
                        if (Enum.TryParse(response.Status, out currentStatus))
                        {
                            if (currentStatus == OrderStatus.Entered)
                            {
                                orderApiClient.SetStatusPosted(orderApiCred, orderId, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _looger.Log(ex.Message);
            }
        }
    }
}