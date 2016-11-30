using BiWell.Payment.Models;
using BiWell.Payment.Helpers;
using System;
using System.Globalization;
using System.Web.Mvc;
using RestSharp;
using BiWell.Payment.Models.Checkout;

namespace BiWell.Payment.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Payment(OrderDetails orderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderDetails.OrderId))
                {
                    throw new InvalidOperationException("Номер заказа не указан");
                }

                int orderId = 0;
                if (!int.TryParse(orderDetails.OrderId, out orderId))
                {
                    throw new InvalidCastException("Некорректный формат номера заказа");
                }

                var apiClient = ByDesignAPIHelper.CreateAPIClient();

                var response = apiClient.GetTotals(apiClient.CreateCredentials(), orderId);
                if (response.Success == 0)
                {
                    throw new InvalidOperationException(response.Message);
                }

                orderDetails.Amount = decimal.Parse(response.BalanceDue, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return View("OrderError", ex);
            }

            return View(orderDetails);
        }

        public ActionResult Delivery(OrderDetails orderDetails)
        {
            DeliveryParameters deliveryParameters = new DeliveryParameters();
            try
            {
                var client = new RestClient(Properties.Settings.Default.CheckoutBaseUrl);
                var request = new RestRequest("service/login/ticket/{apiKey}", Method.GET);
                request.AddUrlSegment("apiKey", Properties.Settings.Default.CheckoutApiKey);

                IRestResponse<TicketResponse> response = client.Execute<TicketResponse>(request);
                TicketResponse ticketResponse = response.Data;

                if (ticketResponse.error)
                {
                    throw new InvalidOperationException("Ошибка получения сессионного ключа: " + ticketResponse.errorMessage);
                }

                deliveryParameters.ticket = ticketResponse.ticket;

                // Getting order items from Freedom

                // Getting order items weight from database 
            }
            catch(Exception ex)
            {
                return View("OrderError", ex);
            }

            return View(deliveryParameters);
        }
    }
}