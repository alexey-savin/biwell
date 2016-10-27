using BiWell.Payment.Helpers;
using System;
using BiWell.Payment.Interfaces;
using BiWell.Payment.Models;

namespace BiWell.Payment.Implementation
{
    public class ByDesignOrderProcessor : IPaymentNotificationProcessor
    {
        private IExceptionLogger _looger = null;
        
        public ByDesignOrderProcessor(IExceptionLogger logger)
        {
            _looger = logger;
        }

        public void ProcessNotification(PaymentNotificationData data)
        {
            try
            {
                var apiClient = ByDesignAPIHelper.CreateAPIClient();

                int orderId = 0;
                if (int.TryParse(data.mnt_transaction_id, out orderId))
                {
                    var credentials = apiClient.CreateCredentials();
                    var response = apiClient.GetOrderInfo(credentials, orderId);

                    if (response.Success > 0)
                    {
                        OrderStatus currentStatus = OrderStatus.Unknown;

                        if (Enum.TryParse(response.Status, out currentStatus))
                        {
                            if (currentStatus == OrderStatus.Entered)
                            {
                                apiClient.SetStatusPosted(credentials, orderId, 0);
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