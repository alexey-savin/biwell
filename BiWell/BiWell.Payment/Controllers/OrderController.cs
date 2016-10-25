using BiWell.Payment.Models;
using BiWell.Payment.Helpers;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace BiWell.Payment.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Payment(PaymentDetails paymentDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentDetails.OrderId))
                {
                    throw new InvalidOperationException("Номер заказа не указан");
                }

                int orderId = 0;
                if (!int.TryParse(paymentDetails.OrderId, out orderId))
                {
                    throw new InvalidCastException("Некорректный формат номера заказа");
                }

                var apiClient = ByDesignAPIHelper.CreateAPIClient();

                var response = apiClient.GetTotals(apiClient.CreateCredentials(), orderId);
                if (response.Success == 0)
                {
                    throw new InvalidOperationException(response.Message);
                }

                paymentDetails.Amount = decimal.Parse(response.BalanceDue, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return View("PaymentError", ex);
            }

            return View(paymentDetails);
        }
    }
}