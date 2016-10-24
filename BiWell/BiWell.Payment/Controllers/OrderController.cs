using BiWell.Payment.Entities;
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

                ByDesignOrderAPI.OrderAPISoap apiClient = new ByDesignOrderAPI.OrderAPISoapClient();
                ByDesignOrderAPI.Credentials cred = new ByDesignOrderAPI.Credentials();
                cred.Username = Properties.Settings.Default.ByDesignApiUser;
                cred.Password = Properties.Settings.Default.ByDesignApiPassword;

                var response = apiClient.GetTotals(cred, orderId);
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